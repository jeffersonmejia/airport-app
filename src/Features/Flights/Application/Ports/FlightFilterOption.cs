namespace Airport.Features.Flights.Application.Ports;

public sealed record AirlineFilterOption(short Id, string Iata, string Name);

public sealed record AirplaneFilterOption(int Id, string Model, int Capacity);

public sealed record FlightRouteFilter(
    int OriginAirportId,
    int DestinationAirportId,
    DateOnly DepartureDate);

public sealed record FlightNumberFilter(
    int OriginAirportId,
    int DestinationAirportId,
    DateOnly? DepartureDate,
    short? AirlineId,
    int? AirplaneId);
