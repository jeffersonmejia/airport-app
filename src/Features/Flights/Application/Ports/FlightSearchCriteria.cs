namespace Airport.Features.Flights.Application.Ports;

public sealed record FlightSearchCriteria(
    int? OriginAirportId,
    int? DestinationAirportId,
    DateOnly? DepartureDate,
    string? Number,
    string SortBy,
    bool Descending,
    short? AirlineId,
    int? AirplaneId);
