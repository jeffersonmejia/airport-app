using Airport.Features.Flights.Domain;

namespace Airport.Features.Flights.Application.Ports;

public sealed record FlightSearchPage(IReadOnlyList<Flight> Items, int TotalItems);
