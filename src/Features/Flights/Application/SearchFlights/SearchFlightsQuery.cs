using Airport.SharedKernel.Pagination;

namespace Airport.Features.Flights.Application.SearchFlights;

public sealed record SearchFlightsQuery(
    int? OriginAirportId,
    int? DestinationAirportId,
    DateOnly? DepartureDate,
    string? Number,
    string SortBy = "departure",
    bool Descending = false,
    int Page = PaginationPolicy.DefaultPage,
    int PageSize = PaginationPolicy.PageSize,
    string? OriginCode = null,
    string? DestinationCode = null,
    short? AirlineId = null,
    int? AirplaneId = null);
