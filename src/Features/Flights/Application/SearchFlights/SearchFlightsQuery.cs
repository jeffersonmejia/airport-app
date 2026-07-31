using Airport.SharedKernel.Pagination;

namespace Airport.Features.Flights.Application.SearchFlights;

public sealed record SearchFlightsQuery(
    string? Number,
    int Page = PaginationPolicy.DefaultPage,
    int PageSize = PaginationPolicy.PageSize);
