using Airport.Features.Bookings.Application.Ports;

namespace Airport.Features.Bookings.Application.CancelBooking;

public sealed class CancelBookingHandler(IBookingRepository repository)
{
    public Task<BookingMutationResult> HandleAsync(
        CancelBookingCommand command,
        CancellationToken cancellationToken) =>
        repository.CancelAsync(
            command.BookingId,
            command.EmployeeId,
            command.Reason.Trim(),
            cancellationToken);
}
