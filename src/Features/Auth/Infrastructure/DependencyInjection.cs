using Airport.Features.Auth.Application.Ports;
using Airport.Features.Auth.Application.Security;
using Airport.Features.Auth.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Airport.Features.Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer), "JWT issuer is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience), "JWT audience is required.")
            .Validate(options => options.SigningKey.Length >= 32, "JWT signing key must have at least 32 characters.")
            .Validate(options => options.AccessTokenMinutes is >= TokenLifetimePolicy.MinimumMinutes
                and <= TokenLifetimePolicy.MaximumMinutes,
                $"JWT lifetime must be between {TokenLifetimePolicy.MinimumMinutes} and {TokenLifetimePolicy.MaximumMinutes} minutes.")
            .Validate(options => options.ClockSkewSeconds is >= 0 and <= 120,
                "JWT clock skew must be between 0 and 120 seconds.")
            .ValidateOnStart();

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IActiveSessionRegistry, MemoryActiveSessionRegistry>();
        services.AddSingleton<IAccessTokenIssuer, JwtAccessTokenIssuer>();

        return services;
    }
}
