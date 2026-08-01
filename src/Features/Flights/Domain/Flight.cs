namespace Airport.Features.Flights.Domain;

public sealed record Flight(
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
    short AirlineId,
    string AirlineName,
    int AirplaneId)
{
    public Flight(
        int id,
        string number,
        DateTimeOffset departure,
        DateTimeOffset arrival,
        short airlineId,
        int airplaneId)
        : this(
            id,
            number,
            0,
            "N/A",
            "Origen",
            0,
            "N/A",
            "Destino",
            departure,
            arrival,
            airlineId,
            $"Aerolínea {airlineId}",
            airplaneId)
    {
    }

    public TimeSpan Duration => Arrival - Departure;

    public decimal BaseFare => decimal.Round(
        45m + Math.Max(1m, (decimal)Duration.TotalHours) * 38m,
        2,
        MidpointRounding.AwayFromZero);

    public IReadOnlyCollection<FareOption> Fares =>
    [
        FareOption.Create("ECONOMY", "Económica", BaseFare, 1m, false, false),
        FareOption.Create("FLEX", "Flexible", BaseFare, 1.35m, true, false),
        FareOption.Create("BUSINESS", "Ejecutiva", BaseFare, 2.10m, true, true)
    ];
}
