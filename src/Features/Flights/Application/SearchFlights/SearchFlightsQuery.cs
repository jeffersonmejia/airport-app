namespace Airport.Features.Flights.Application.SearchFlights;

public sealed record SearchFlightsQuery(string? Number, int Page = 1, int PageSize = 8);
