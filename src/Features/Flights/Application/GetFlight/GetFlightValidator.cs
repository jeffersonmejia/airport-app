namespace Airport.Features.Flights.Application.GetFlight;

public sealed class GetFlightValidator
{
    public string? Validate(GetFlightQuery query) =>
        query.FlightId > 0 ? null : "El identificador del vuelo debe ser mayor que cero.";
}
