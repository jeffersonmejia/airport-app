namespace Airport.Infrastructure.Features.Flights;

public sealed class FlightRow
{
    public int FlightId { get; init; }

    public string FlightNumber { get; init; } = string.Empty;

    public DateTimeOffset Departure { get; init; }

    public DateTimeOffset Arrival { get; init; }

    public short AirlineId { get; init; }

    public int AirplaneId { get; init; }
}
