namespace HubCinema.Services.Dtos.Auth;

/// <summary>
/// Returned on register / login / refresh. The refresh token's raw value is included
/// here once — only its SHA-256 hash is persisted server-side.
/// </summary>
public record AuthResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt,
    UserInfo User);

public record UserInfo(int Id, string FullName, string Email, string Role);
