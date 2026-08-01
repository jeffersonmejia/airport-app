using Airport.Features.Flights.Application.ListFilterOptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Airport.Features.Flights.Presentation.Api.ListFilterOptions;

public static class ListFilterOptionsEndpoints
{
    public static RouteGroupBuilder MapListFilterOptions(this RouteGroupBuilder group)
    {
        group.MapGet("/airlines", async (
                ListFilterOptionsHandler handler,
                CancellationToken cancellationToken) =>
            Results.Ok(await handler.ListAirlinesAsync(cancellationToken)))
            .WithName("ListFlightAirlines")
            .WithSummary("Lista aerolíneas con vuelos disponibles")
            .Produces<IReadOnlyCollection<AirlineOptionResponse>>();

        group.MapGet("/airplanes", HandleAirplanesAsync)
            .WithName("ListFlightAirplanes")
            .WithSummary("Lista aviones disponibles para una aerolínea")
            .Produces<IReadOnlyCollection<AirplaneOptionResponse>>()
            .ProducesValidationProblem();

        group.MapGet("/numbers", HandleFlightNumbersAsync)
            .WithName("ListFlightNumbers")
            .WithSummary("Lista números de vuelo compatibles con los filtros actuales")
            .Produces<IReadOnlyList<string>>()
            .ProducesValidationProblem();

        return group;
    }

    private static async Task<IResult> HandleAirplanesAsync(
        short? airlineId,
        ListFilterOptionsHandler handler,
        CancellationToken cancellationToken)
    {
        if (airlineId is null or <= 0)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(airlineId)] = ["Selecciona una aerolínea válida."]
            });
        }

        return Results.Ok(await handler.ListAirplanesAsync(airlineId.Value, cancellationToken));
    }

    private static async Task<IResult> HandleFlightNumbersAsync(
        int? originAirportId,
        int? destinationAirportId,
        DateOnly? departureDate,
        short? airlineId,
        int? airplaneId,
        ListFilterOptionsHandler handler,
        CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();
        if (originAirportId is null or <= 0)
        {
            errors[nameof(originAirportId)] = ["Selecciona un aeropuerto de origen válido."];
        }

        if (destinationAirportId is null or <= 0)
        {
            errors[nameof(destinationAirportId)] = ["Selecciona un aeropuerto de destino válido."];
        }

        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        return Results.Ok(await handler.ListFlightNumbersAsync(
            originAirportId!.Value,
            destinationAirportId!.Value,
            departureDate,
            airlineId,
            airplaneId,
            cancellationToken));
    }
}
