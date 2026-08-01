using Airport.Features.Flights.Application.ListAirports;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Airport.Features.Flights.Presentation.Api.ListAirports;

public static class ListAirportsEndpoint
{
    public static RouteGroupBuilder MapListAirports(this RouteGroupBuilder group)
    {
        group.MapGet("/airports", HandleAsync)
            .WithName("ListAirports")
            .WithSummary("Lista orígenes o destinos disponibles para buscar vuelos")
            .Produces<IReadOnlyCollection<AirportResponse>>()
            .ProducesValidationProblem();

        return group;
    }

    private static async Task<IResult> HandleAsync(
        int? originAirportId,
        ListAirportsHandler handler,
        CancellationToken cancellationToken)
    {
        if (originAirportId is <= 0)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(originAirportId)] = ["El aeropuerto de origen no es válido."]
            });
        }

        return Results.Ok(await handler.HandleAsync(originAirportId, cancellationToken));
    }
}
