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
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));

        return services;
    }
}
