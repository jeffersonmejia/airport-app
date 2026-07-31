namespace Airport.Features.Bookings.Application.SearchBookings;

public sealed record SearchBookingsResponse(
    IReadOnlyCollection<BookingResponse> Items,
    int Page,
    int PageSize,
    bool HasNextPage,
    int TotalItems,
    int TotalPages,
    bool TotalApproximate);
