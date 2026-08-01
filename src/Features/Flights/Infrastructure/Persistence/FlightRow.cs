namespace Airport.Features.Flights.Infrastructure.Persistence;

public sealed class FlightRow
{
    public int FlightId { get; init; }

    public string FlightNumber { get; init; } = string.Empty;

    public int OriginAirportId { get; init; }

    public AirportRow OriginAirport { get; init; } = default!;

    public int DestinationAirportId { get; init; }

    public AirportRow DestinationAirport { get; init; } = default!;

    public DateTimeOffset Departure { get; init; }

    public DateTimeOffset Arrival { get; init; }

    public short AirlineId { get; init; }

    public int AirplaneId { get; init; }
}
