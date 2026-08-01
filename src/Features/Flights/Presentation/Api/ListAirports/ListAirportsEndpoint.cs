using Airport.Features.Flights.Application.ListAirports;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Airport.Features.Flights.Presentation.Api.ListAirports;

public static class ListAirportsEndpoint
{
    public static RouteGroupBuilder MapListAirports(this RouteGroupBuilder group)
    {
        group.MapGet("/airports", async (
                ListAirportsHandler handler,
                CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(cancellationToken)))
            .WithName("ListAirports")
            .WithSummary("Lista aeropuertos disponibles para buscar vuelos")
            .Produces<IReadOnlyCollection<AirportResponse>>();

        return group;
    }
}
