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

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    // Computed helpers (not mapped to columns).
    public bool IsRevoked => RevokedAt is not null;

    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
}
