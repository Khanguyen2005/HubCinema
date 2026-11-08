using HubCinema.Repositories.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HubCinema.Repositories;

/// <summary>
/// Composition root for the data-access layer. The API calls AddDataAccess(...)
/// so the Presentation layer never references EF Core directly.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'Default' is not configured. Set ConnectionStrings:Default in " +
                "appsettings.Development.json (local) or an environment variable before starting the app.");
        }

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

        return services;
    }
}
