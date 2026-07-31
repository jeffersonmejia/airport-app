using Airport.Features.Auth.Presentation.Web.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Airport.Features.Auth.Presentation.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthPresentation(this IServiceCollection services)
    {
        services.AddScoped<AuthClient>();
        services.AddScoped<AuthSession>();
        return services;
    }
}
