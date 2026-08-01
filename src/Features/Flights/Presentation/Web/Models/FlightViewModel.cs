namespace Airport.Features.Flights.Presentation.Web.Models;

public sealed record FlightViewModel(
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
    string? AirlineName,
    int AirplaneId,
    decimal FromPrice,
    IReadOnlyCollection<FareViewModel>? Fares = null);

public sealed record FareViewModel(
    string Code,
    string Name,
    decimal Price,
    bool AllowsChanges,
    bool PriorityBoarding);

public sealed record FlightSearchInput(
    string? OriginCode,
    string? DestinationCode,
    DateOnly? DepartureDate,
    string? Number,
    short? AirlineId,
    int? AirplaneId,
    string SortBy,
    bool Descending);
