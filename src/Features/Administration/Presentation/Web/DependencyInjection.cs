using Airport.Features.Administration.Presentation.Web.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Airport.Features.Administration.Presentation.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddAdministrationPresentation(
        this IServiceCollection services)
    {
        services.AddScoped<AdministrationClient>();
        return services;
    }
}
