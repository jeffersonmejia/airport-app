using Airport.SharedKernel.Pagination;

namespace Airport.Features.Bookings.Application.SearchBookings;

public sealed record SearchBookingsQuery(
    int? BookingId,
    int? FlightId,
    int? PassengerId,
    int Page = PaginationPolicy.DefaultPage,
    int PageSize = PaginationPolicy.PageSize);
