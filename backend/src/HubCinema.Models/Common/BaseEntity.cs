namespace HubCinema.Models.Common;

/// <summary>
/// Base type for all persistent entities. Centralizes the surrogate key,
/// audit timestamps, and the soft-delete flag so every table gets them
/// consistently. CreatedAt/UpdatedAt are stamped automatically in AppDbContext.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    /// <summary>Soft delete marker. Rows with IsDeleted = true are filtered out by a global query filter.</summary>
    public bool IsDeleted { get; set; }
}
