using HubCinema.Models.Entities;

namespace HubCinema.Services.Interfaces;

/// <summary>A signed JWT access token and the moment it expires.</summary>
public record AccessToken(string Value, DateTime ExpiresAt);

/// <summary>
/// A newly minted refresh token: the raw value (returned to the client once), its hash
/// (persisted), and its expiry.
/// </summary>
public record RefreshTokenResult(string RawValue, string Hash, DateTime ExpiresAt);

public interface ITokenService
{
    AccessToken CreateAccessToken(User user);

    RefreshTokenResult CreateRefreshToken();

    /// <summary>SHA-256 hash of a raw refresh token — used to persist it and to look it up on refresh.</summary>
    string HashRefreshToken(string rawToken);
}
