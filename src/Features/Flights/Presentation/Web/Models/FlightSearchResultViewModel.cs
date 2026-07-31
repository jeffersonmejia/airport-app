namespace Airport.Features.Flights.Presentation.Web.Models;

public sealed record FlightSearchResultViewModel(
    IReadOnlyList<FlightViewModel> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);
