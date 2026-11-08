using FluentValidation;
using HubCinema.Services.Implementations;
using HubCinema.Services.Interfaces;
using HubCinema.Services.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HubCinema.Services;

/// <summary>
/// Composition root for the application/service layer. The API calls AddApplicationServices(...)
/// so it doesn't need to know about individual service implementations.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(JwtSettings.SectionName))
            .Validate(
                s => !string.IsNullOrWhiteSpace(s.SecretKey) && s.SecretKey.Length >= 32,
                "Jwt:SecretKey is missing or shorter than 32 characters. Set it in appsettings.Development.json (local) or an environment variable.")
            .Validate(s => !string.IsNullOrWhiteSpace(s.Issuer), "Jwt:Issuer must not be empty.")
            .Validate(s => !string.IsNullOrWhiteSpace(s.Audience), "Jwt:Audience must not be empty.")
            .Validate(s => s.AccessTokenMinutes > 0, "Jwt:AccessTokenMinutes must be greater than 0.")
            .Validate(s => s.RefreshTokenDays > 0, "Jwt:RefreshTokenDays must be greater than 0.")
            .ValidateOnStart();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddValidatorsFromAssemblyContaining<AuthService>();

        return services;
    }
}
