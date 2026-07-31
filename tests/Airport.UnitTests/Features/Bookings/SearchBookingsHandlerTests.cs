using Airport.Features.Bookings.Application.Ports;
using Airport.Features.Bookings.Application.SearchBookings;
using Airport.Features.Bookings.Domain;

namespace Airport.UnitTests.Bookings;

public sealed class SearchBookingsHandlerTests
{
    [Fact]
    public async Task HandleAsync_MapsItemsAndCalculatesTotalPages()
    {
        var bookings = new[]
        {
            new Booking(7, 1, 1, "1A", 120m, new DateTimeOffset(2026, 7, 30, 8, 0, 0, TimeSpan.Zero), false, 1)
        };
        var handler = new SearchBookingsHandler(
            new StubBookingRepository(bookings, 17),
            TimeProvider.System);

        var result = await handler.HandleAsync(
            new SearchBookingsQuery(null, null, null, 2, 5),
            CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(7, result.Items.First().BookingId);
        Assert.Equal(4, result.TotalPages);
        Assert.Equal(17, result.TotalItems);
    }

    private sealed class StubBookingRepository(
        IReadOnlyCollection<Booking> bookings,
        int totalItems) : IBookingRepository
    {
        public Task<BookingPage> SearchAsync(
            int? bookingId,
            int? flightId,
            int? passengerId,
            int page,
            int pageSize,
            CancellationToken cancellationToken) =>
            Task.FromResult(new BookingPage(bookings, page, false, totalItems, false));

        public Task<Booking?> FindByIdAsync(int bookingId, CancellationToken cancellationToken) =>
            Task.FromResult<Booking?>(null);

        public Task<BookingMutationResult> CreateAsync(
            int flightId,
            int passengerId,
            string? seat,
            decimal price,
            CancellationToken cancellationToken) =>
            Task.FromResult(new BookingMutationResult(BookingMutationStatus.Success));

        public Task<BookingMutationResult> UpdateAsync(
            int bookingId,
            string? seat,
            decimal price,
            uint version,
            CancellationToken cancellationToken) =>
            Task.FromResult(new BookingMutationResult(BookingMutationStatus.Success));

        public Task<BookingMutationResult> CancelAsync(
            int bookingId,
            int employeeId,
            string reason,
            CancellationToken cancellationToken) =>
            Task.FromResult(new BookingMutationResult(BookingMutationStatus.Success));
    }
}
