namespace Airport.Core.Flights.Domain;

public sealed record Flight(
    int Id,
    string Number,
    DateTimeOffset Departure,
    DateTimeOffset Arrival,
    short AirlineId,
    int AirplaneId)
{
    public TimeSpan Duration => Arrival - Departure;
}
