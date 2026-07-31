namespace Airport.Features.Bookings.Presentation.Web.Models;

public sealed record BookingSearchViewModel(
    IReadOnlyCollection<BookingViewModel> Items,
    int Page,
    int PageSize,
    bool HasNextPage,
    int TotalItems,
    int TotalPages,
    bool TotalApproximate);
