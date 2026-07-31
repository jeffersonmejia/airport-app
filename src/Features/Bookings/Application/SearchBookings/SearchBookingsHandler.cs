using Airport.Features.Bookings.Application.Ports;

namespace Airport.Features.Bookings.Application.SearchBookings;

public sealed class SearchBookingsHandler(IBookingRepository repository, TimeProvider timeProvider)
{
    public async Task<SearchBookingsResponse> HandleAsync(
        SearchBookingsQuery query,
        CancellationToken cancellationToken)
    {
        var page = await repository.SearchAsync(
            query.BookingId,
            query.FlightId,
            query.PassengerId,
            query.Page,
            query.PageSize,
            cancellationToken);
        var now = timeProvider.GetUtcNow();
        var totalPages = page.TotalItems == 0
            ? 0
            : (int)Math.Ceiling(page.TotalItems / (double)query.PageSize);

        return new SearchBookingsResponse(
            page.Items.Select(item => BookingResponse.FromDomain(item, now)).ToArray(),
            page.Page,
            query.PageSize,
            page.HasNextPage,
            page.TotalItems,
            totalPages,
            page.TotalApproximate);
    }
}
