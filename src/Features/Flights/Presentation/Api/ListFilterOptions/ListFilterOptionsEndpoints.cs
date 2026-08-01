using Airport.Features.Flights.Application.ListFilterOptions;
using Airport.Features.Flights.Application.Ports;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Airport.Features.Flights.Presentation.Api.ListFilterOptions;

public static class ListFilterOptionsEndpoints
{
    public static RouteGroupBuilder MapListFilterOptions(this RouteGroupBuilder group)
    {
        group.MapGet("/dates", HandleDepartureDatesAsync)
            .WithName("ListFlightDepartureDates")
            .WithSummary("Lista fechas con vuelos para una ruta")
            .Produces<IReadOnlyList<DateOnly>>()
            .ProducesValidationProblem();

        group.MapGet("/airlines", HandleAirlinesAsync)
            .WithName("ListFlightAirlines")
            .WithSummary("Lista aerolíneas disponibles para una ruta y fecha")
            .Produces<IReadOnlyCollection<AirlineOptionResponse>>()
            .ProducesValidationProblem();

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
        int? originAirportId,
        int? destinationAirportId,
        DateOnly? departureDate,
        ListFilterOptionsHandler handler,
        CancellationToken cancellationToken)
    {
        var errors = ValidateRoute(originAirportId, destinationAirportId, departureDate);
        if (airlineId is null or <= 0)
        {
            errors[nameof(airlineId)] = ["Selecciona una aerolínea válida."];
        }

        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        return Results.Ok(await handler.ListAirplanesAsync(
            airlineId!.Value,
            CreateRoute(originAirportId, destinationAirportId, departureDate),
            cancellationToken));
    }

    private static async Task<IResult> HandleDepartureDatesAsync(
        int? originAirportId,
        int? destinationAirportId,
        ListFilterOptionsHandler handler,
        CancellationToken cancellationToken)
    {
        var errors = ValidateRoute(originAirportId, destinationAirportId, null, false);
        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        return Results.Ok(await handler.ListDepartureDatesAsync(
            originAirportId!.Value,
            destinationAirportId!.Value,
            cancellationToken));
    }

    private static async Task<IResult> HandleAirlinesAsync(
        int? originAirportId,
        int? destinationAirportId,
        DateOnly? departureDate,
        ListFilterOptionsHandler handler,
        CancellationToken cancellationToken)
    {
        var errors = ValidateRoute(originAirportId, destinationAirportId, departureDate);
        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        return Results.Ok(await handler.ListAirlinesAsync(
            CreateRoute(originAirportId, destinationAirportId, departureDate),
            cancellationToken));
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
        var errors = ValidateRoute(originAirportId, destinationAirportId, departureDate);
        if (airlineId is <= 0)
        {
            errors[nameof(airlineId)] = ["La aerolínea seleccionada no es válida."];
        }

        if (airplaneId is <= 0)
        {
            errors[nameof(airplaneId)] = ["El avión seleccionado no es válido."];
        }

        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        return Results.Ok(await handler.ListFlightNumbersAsync(
            originAirportId!.Value,
            destinationAirportId!.Value,
            departureDate!.Value,
            airlineId,
            airplaneId,
            cancellationToken));
    }

    private static Dictionary<string, string[]> ValidateRoute(
        int? originAirportId,
        int? destinationAirportId,
        DateOnly? departureDate,
        bool requireDate = true)
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

        if (requireDate && departureDate is null)
        {
            errors[nameof(departureDate)] = ["Selecciona una fecha disponible."];
        }

        return errors;
    }

    private static FlightRouteFilter CreateRoute(
        int? originAirportId,
        int? destinationAirportId,
        DateOnly? departureDate) =>
        new(originAirportId!.Value, destinationAirportId!.Value, departureDate!.Value);
}
