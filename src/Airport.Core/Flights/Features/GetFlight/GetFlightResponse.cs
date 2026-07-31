using Airport.Core.Flights.Domain;

namespace Airport.Core.Flights.Features.GetFlight;

public sealed record GetFlightResponse(
    int Id,
    string Number,
    DateTimeOffset Departure,
    DateTimeOffset Arrival,
    double DurationHours,
    short AirlineId,
    int AirplaneId)
{
    public static GetFlightResponse FromDomain(Flight flight) => new(
        flight.Id,
        flight.Number.Trim(),
        flight.Departure,
        flight.Arrival,
        Math.Round(flight.Duration.TotalHours, 2),
        flight.AirlineId,
        flight.AirplaneId);
}
