using Airport.Features.Bookings.Domain;

namespace Airport.UnitTests.Bookings;

public sealed class BookingTests
{
    [Fact]
    public void FutureActiveBookingCanBeModified()
    {
        var now = DateTimeOffset.UtcNow;
        var booking = CreateBooking(now.AddHours(1), isCancelled: false);

        Assert.True(booking.CanModify(now));
    }

    [Theory]
    [InlineData(true, 1)]
    [InlineData(false, -1)]
    public void CancelledOrHistoricalBookingCannotBeModified(
        bool isCancelled,
        int departureOffsetHours)
    {
        var now = DateTimeOffset.UtcNow;
        var booking = CreateBooking(now.AddHours(departureOffsetHours), isCancelled);

        Assert.False(booking.CanModify(now));
    }

    private static Booking CreateBooking(DateTimeOffset departure, bool isCancelled) =>
        new(1, 1, 1, "1A", 100m, departure, isCancelled, 1);
}
