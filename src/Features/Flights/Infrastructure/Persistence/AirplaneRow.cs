namespace Airport.Features.Flights.Infrastructure.Persistence;

public sealed class AirplaneRow
{
    public int AirplaneId { get; init; }

    public int Capacity { get; init; }

    public int TypeId { get; init; }

    public AirplaneTypeRow Type { get; init; } = default!;

    public short AirlineId { get; init; }
}

public sealed class AirplaneTypeRow
{
    public int TypeId { get; init; }

    public string Identifier { get; init; } = string.Empty;

    public string? Description { get; init; }
}
