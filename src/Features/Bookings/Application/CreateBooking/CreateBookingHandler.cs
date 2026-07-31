using Airport.Features.Bookings.Application.Ports;

namespace Airport.Features.Bookings.Application.CreateBooking;

public sealed class CreateBookingHandler(IBookingRepository repository)
{
    public Task<BookingMutationResult> HandleAsync(
        CreateBookingCommand command,
        CancellationToken cancellationToken) =>
        repository.CreateAsync(
            command.FlightId,
            command.PassengerId,
            BookingValidation.NormalizeSeat(command.Seat),
            command.Price,
            cancellationToken);
}
