namespace Airport.Features.Flights.Domain;

public sealed record FareOption(
    string Code,
    string Name,
    decimal Price,
    bool AllowsChanges,
    bool PriorityBoarding)
{
    public static FareOption Create(
        string code,
        string name,
        decimal baseFare,
        decimal multiplier,
        bool allowsChanges,
        bool priorityBoarding) => new(
            code,
            name,
            decimal.Round(baseFare * multiplier, 2, MidpointRounding.AwayFromZero),
            allowsChanges,
            priorityBoarding);
}
