using Airport.Features.Flights.Domain;

namespace Airport.Features.Flights.Application.SearchFlights;

public sealed record SearchFlightItemResponse(
    int Id,
    string Number,
    DateTimeOffset Departure,
    DateTimeOffset Arrival,
    double DurationHours,
    short AirlineId,
    int AirplaneId)
{
    public static SearchFlightItemResponse FromDomain(Flight flight) => new(
        flight.Id,
        flight.Number.Trim(),
        flight.Departure,
        flight.Arrival,
        Math.Round(flight.Duration.TotalHours, 2),
        flight.AirlineId,
        flight.AirplaneId);
}

public sealed record SearchFlightsResponse(
    IReadOnlyList<SearchFlightItemResponse> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);
