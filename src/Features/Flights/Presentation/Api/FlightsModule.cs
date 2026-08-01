using Airport.Features.Flights.Application.GetFlight;
using Airport.Features.Flights.Application.SearchFlights;
using Airport.Features.Flights.Application.ListAirports;
using Airport.Features.Flights.Infrastructure;
using Airport.Features.Flights.Presentation.Api.GetFlight;
using Airport.Features.Flights.Presentation.Api.SearchFlights;
using Airport.Features.Flights.Presentation.Api.ListAirports;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Airport.Features.Flights.Presentation.Api;

public static class FlightsModule
{
    public static IServiceCollection AddFlightsModule(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddSingleton<GetFlightValidator>();
        services.AddScoped<GetFlightHandler>();
        services.AddSingleton<SearchFlightsValidator>();
        services.AddScoped<SearchFlightsHandler>();
        services.AddScoped<ListAirportsHandler>();
        services.AddFlightsInfrastructure(connectionString);

        return services;
    }

    public static IEndpointRouteBuilder MapFlightsModule(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGroup("/api/flights")
            .MapGetFlight()
            .MapListAirports()
            .MapSearchFlights();

        return endpoints;
    }
}
