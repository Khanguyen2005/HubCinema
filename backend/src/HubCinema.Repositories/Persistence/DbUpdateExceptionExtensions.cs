using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HubCinema.Repositories.Persistence;

/// <summary>Helpers for interpreting <see cref="DbUpdateException"/> from SQL Server.</summary>
public static class DbUpdateExceptionExtensions
{
    // SQL Server error numbers: 2627 = unique constraint violation, 2601 = duplicate key in a unique index.
    private const int UniqueConstraintError = 2627;
    private const int DuplicateKeyError = 2601;

    /// <summary>True when the save failed because it violated a unique index/constraint.</summary>
    public static bool IsUniqueConstraintViolation(this DbUpdateException exception)
        => exception.InnerException is SqlException { Number: UniqueConstraintError or DuplicateKeyError };
}
