using HubCinema.Models.Common;

namespace HubCinema.Models.Entities;

/// <summary>
/// Persisted refresh token for stateless JWT auth. The short-lived access token
/// is never stored; only refresh tokens live here so they can be rotated/revoked.
/// </summary>
public class RefreshToken : BaseEntity
{
    public int UserId { get; set; }

    public User User { get; set; } = null!;

    /// <summary>
    /// SHA-256 hash of the refresh token — never the raw value. The raw token is
    /// returned to the client once at issue time; only its hash is persisted, so a
    /// read-only DB compromise cannot reuse it. Look up tokens by this hash.
    /// </summary>
    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    // Computed helpers (not mapped to columns).
    public bool IsRevoked => RevokedAt is not null;

    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
}
