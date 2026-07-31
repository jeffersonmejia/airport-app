using Airport.Features.Flights.Application.GetFlight;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Airport.Features.Flights.Presentation.Api.GetFlight;

public static class GetFlightEndpoint
{
    public static RouteGroupBuilder MapGetFlight(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}", HandleAsync)
            .WithName("GetFlight")
            .WithSummary("Obtiene un vuelo por su identificador")
            .Produces<GetFlightResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<IResult> HandleAsync(
        int id,
        GetFlightValidator validator,
        GetFlightHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetFlightQuery(id);
        var validationError = validator.Validate(query);

        if (validationError is not null)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(id)] = [validationError]
            });
        }

        var response = await handler.HandleAsync(query, cancellationToken);
        return response is null
            ? Results.Problem(
                title: "Vuelo no encontrado",
                detail: $"No existe un vuelo con identificador {id}.",
                statusCode: StatusCodes.Status404NotFound)
            : Results.Ok(response);
    }
}
