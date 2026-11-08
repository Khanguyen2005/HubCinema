using HubCinema.Common.Exceptions;
using HubCinema.Models.Entities;
using HubCinema.Models.Enums;
using HubCinema.Repositories.Persistence;
using HubCinema.Services.Dtos.Auth;
using HubCinema.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HubCinema.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(AppDbContext db, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var phone = request.PhoneNumber.Trim();

        // Check against ALL users, soft-deleted included: the unique indexes span them too, so
        // email/phone stay globally unique. Ignoring the global filter here turns a soft-deleted
        // collision into a clean 409 instead of a unique-violation at save time.
        var exists = await _db.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Email == email || u.PhoneNumber == phone, cancellationToken);
        if (exists)
        {
            throw new ConflictException("Email or phone number is already registered.");
        }

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = email,
            PhoneNumber = phone,
            PasswordHash = _passwordHasher.Hash(request.Password),
            DateOfBirth = request.DateOfBirth,
            Role = UserRole.Customer,
        };

        _db.Users.Add(user);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
        {
            // Two concurrent registrations can both pass the pre-check; the unique index is the
            // source of truth and rejects the loser. Surface it as a conflict, not a 500.
            throw new ConflictException("Email or phone number is already registered.");
        }

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        // Same message whether the email or the password is wrong — don't reveal which.
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken = default)
    {
        var hash = _tokenService.HashRefreshToken(request.RefreshToken);

        var token = await _db.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);

        // Reject if the token is unknown, inactive, or owned by a soft-deleted user
        // (the global query filter leaves User null when the owner is soft-deleted).
        if (token is null || !token.IsActive || token.User is null)
        {
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        var now = DateTime.UtcNow;

        // Rotate inside one transaction. The conditional UPDATE flips RevokedAt only while the
        // token is still un-revoked AND unexpired, so of several concurrent requests sharing one
        // token exactly one wins (replay-safe) and expiry is enforced at redemption time. Issuing
        // the replacement in the same transaction means we never revoke without re-issuing.
        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        var revoked = await _db.RefreshTokens
            .Where(t => t.Id == token.Id && t.RevokedAt == null && t.ExpiresAt > now)
            .ExecuteUpdateAsync(
                s => s.SetProperty(t => t.RevokedAt, now).SetProperty(t => t.UpdatedAt, now),
                cancellationToken);

        if (revoked == 0)
        {
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        var response = await IssueTokensAsync(token.User, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return response;
    }

    public async Task LogoutAsync(RefreshRequest request, CancellationToken cancellationToken = default)
    {
        var hash = _tokenService.HashRefreshToken(request.RefreshToken);
        var token = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);

        // Idempotent: revoke if we hold an active token; otherwise do nothing.
        if (token is { RevokedAt: null })
        {
            token.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<AuthResponse> IssueTokensAsync(User user, CancellationToken cancellationToken)
    {
        var access = _tokenService.CreateAccessToken(user);
        var refresh = _tokenService.CreateRefreshToken();

        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refresh.Hash,
            ExpiresAt = refresh.ExpiresAt,
        });

        await _db.SaveChangesAsync(cancellationToken);

        var userInfo = new UserInfo(user.Id, user.FullName, user.Email, user.Role.ToString());
        return new AuthResponse(access.Value, access.ExpiresAt, refresh.RawValue, refresh.ExpiresAt, userInfo);
    }
}
