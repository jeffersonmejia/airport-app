using Airport.Features.Bookings.Application.Ports;

namespace Airport.Features.Bookings.Application.UpdateBooking;

public sealed class UpdateBookingHandler(IBookingRepository repository)
{
    public Task<BookingMutationResult> HandleAsync(
        UpdateBookingCommand command,
        CancellationToken cancellationToken) =>
        repository.UpdateAsync(
            command.BookingId,
            BookingValidation.NormalizeSeat(command.Seat),
            command.Price,
            command.Version,
            cancellationToken);
}
