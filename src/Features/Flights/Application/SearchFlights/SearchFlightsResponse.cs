using Airport.Features.Flights.Domain;

namespace Airport.Features.Flights.Application.SearchFlights;

public sealed record SearchFlightItemResponse(
    int Id,
    string Number,
    int OriginAirportId,
    string OriginCode,
    string OriginName,
    int DestinationAirportId,
    string DestinationCode,
    string DestinationName,
    DateTimeOffset Departure,
    DateTimeOffset Arrival,
    double DurationHours,
    short AirlineId,
    int AirplaneId,
    decimal FromPrice)
{
    public static SearchFlightItemResponse FromDomain(Flight flight) => new(
        flight.Id,
        flight.Number.Trim(),
        flight.OriginAirportId,
        flight.OriginCode,
        flight.OriginName,
        flight.DestinationAirportId,
        flight.DestinationCode,
        flight.DestinationName,
        flight.Departure,
        flight.Arrival,
        Math.Round(flight.Duration.TotalHours, 2),
        flight.AirlineId,
        flight.AirplaneId,
        flight.BaseFare);
}

public sealed record SearchFlightsResponse(
    IReadOnlyList<SearchFlightItemResponse> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);
