namespace HubCinema.Services.Options;

/// <summary>
/// JWT options bound from the "Jwt" config section. <see cref="SecretKey"/> is a secret:
/// it lives in appsettings.Development.json (gitignored) or an environment variable, never committed.
/// </summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    /// <summary>HMAC-SHA256 signing key. Must be at least 32 characters.</summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>Access-token lifetime in minutes (short-lived).</summary>
    public int AccessTokenMinutes { get; set; } = 15;

    /// <summary>Refresh-token lifetime in days.</summary>
    public int RefreshTokenDays { get; set; } = 7;
}
