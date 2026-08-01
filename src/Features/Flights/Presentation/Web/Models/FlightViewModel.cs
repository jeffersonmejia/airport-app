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

public sealed record AirportViewModel(int Id, string Iata, string Icao, string Name)
{
    public string Label => $"{Name} ({Iata})";
}

public sealed record AirlineOptionViewModel(short Id, string Iata, string Name)
{
    public string Label => string.IsNullOrWhiteSpace(Iata) ? Name : $"{Name} ({Iata})";
}

public sealed record AirplaneOptionViewModel(int Id, string Model, int Capacity)
{
    public string Label => $"{Model} · {Capacity} pasajeros";
}

public sealed record FlightSearchInput(
    int? OriginAirportId,
    int? DestinationAirportId,
    DateOnly? DepartureDate,
    string? Number,
    short? AirlineId,
    int? AirplaneId,
    string SortBy,
    bool Descending);
