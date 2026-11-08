using HubCinema.Models.Common;
using HubCinema.Models.Enums;

namespace HubCinema.Models.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    /// <summary>BCrypt hash. Never store plain passwords.</summary>
    public string PasswordHash { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public UserRole Role { get; set; } = UserRole.Customer;

    /// <summary>Loyalty points accrued from bookings.</summary>
    public int LoyaltyPoints { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
