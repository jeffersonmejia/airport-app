namespace Airport.Features.Flights.Infrastructure.Persistence;

public sealed class AirlineRow
{
    public short AirlineId { get; init; }

    public string Iata { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;
}
