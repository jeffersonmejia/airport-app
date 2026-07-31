using Airport.Features.Bookings.Domain;

namespace Airport.Features.Bookings.Application.Ports;

public interface IBookingRepository
{
    Task<BookingPage> SearchAsync(
        int? bookingId,
        int? flightId,
        int? passengerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<Booking?> FindByIdAsync(int bookingId, CancellationToken cancellationToken);

    Task<BookingMutationResult> CreateAsync(
        int flightId,
        int passengerId,
        string? seat,
        decimal price,
        CancellationToken cancellationToken);

    Task<BookingMutationResult> UpdateAsync(
        int bookingId,
        string? seat,
        decimal price,
        uint version,
        CancellationToken cancellationToken);

    Task<BookingMutationResult> CancelAsync(
        int bookingId,
        int employeeId,
        string reason,
        CancellationToken cancellationToken);
}
