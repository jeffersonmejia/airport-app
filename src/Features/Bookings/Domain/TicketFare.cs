namespace Airport.Features.Bookings.Domain;

public sealed record TicketFare(string Code, string Name, decimal Price)
{
    public static TicketFare? FromFlight(string code, DateTimeOffset departure, DateTimeOffset arrival)
    {
        var duration = arrival - departure;
        var baseFare = decimal.Round(
            45m + Math.Max(1m, (decimal)duration.TotalHours) * 38m,
            2,
            MidpointRounding.AwayFromZero);

        return (code ?? string.Empty).Trim().ToUpperInvariant() switch
        {
            "ECONOMY" => new("ECONOMY", "Económica", baseFare),
            "FLEX" => new("FLEX", "Flexible", decimal.Round(baseFare * 1.35m, 2)),
            "BUSINESS" => new("BUSINESS", "Ejecutiva", decimal.Round(baseFare * 2.10m, 2)),
            _ => null
        };
    }
}
