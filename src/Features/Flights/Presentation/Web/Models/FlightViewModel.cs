namespace Airport.Features.Flights.Presentation.Web.Models;

public sealed record FlightViewModel(
    int Id,
    string Number,
    DateTimeOffset Departure,
    DateTimeOffset Arrival,
    double DurationHours,
    short AirlineId,
    int AirplaneId);
