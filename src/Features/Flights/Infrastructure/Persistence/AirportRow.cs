namespace Airport.Features.Flights.Infrastructure.Persistence;

public sealed class AirportRow
{
    public int AirportId { get; init; }

    public string? Iata { get; init; }

    public string Icao { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;
}
