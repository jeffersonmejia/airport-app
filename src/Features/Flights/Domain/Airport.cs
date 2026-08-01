namespace Airport.Features.Flights.Domain;

public sealed record Airport(int Id, string Iata, string Icao, string Name);
