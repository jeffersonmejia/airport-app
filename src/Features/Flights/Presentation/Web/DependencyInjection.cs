using Airport.Features.Flights.Presentation.Web.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Airport.Features.Flights.Presentation.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddFlightsPresentation(this IServiceCollection services)
    {
        services.AddScoped<FlightsClient>();
        return services;
    }
}
