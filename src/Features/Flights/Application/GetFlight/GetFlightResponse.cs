using Airport.Features.Flights.Domain;

namespace Airport.Features.Flights.Application.GetFlight;

public sealed record GetFlightResponse(
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
    string AirlineName,
    int AirplaneId,
    decimal FromPrice,
    IReadOnlyCollection<FareResponse> Fares)
{
    public static GetFlightResponse FromDomain(Flight flight) => new(
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
        flight.AirlineName,
        flight.AirplaneId,
        flight.BaseFare,
        flight.Fares.Select(FareResponse.FromDomain).ToArray());
}

public sealed record FareResponse(
    string Code,
    string Name,
    decimal Price,
    bool AllowsChanges,
    bool PriorityBoarding)
{
    public static FareResponse FromDomain(FareOption fare) => new(
        fare.Code,
        fare.Name,
        fare.Price,
        fare.AllowsChanges,
        fare.PriorityBoarding);
}
