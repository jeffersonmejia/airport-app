using Airport.Features.Bookings.Application.Ports;

namespace Airport.Features.Bookings.Application.GetBooking;

public sealed class GetBookingHandler(IBookingRepository repository, TimeProvider timeProvider)
{
    public async Task<BookingResponse?> HandleAsync(
        int bookingId,
        CancellationToken cancellationToken)
    {
        var booking = await repository.FindByIdAsync(bookingId, cancellationToken);
        return booking is null
            ? null
            : BookingResponse.FromDomain(booking, timeProvider.GetUtcNow());
    }
}
