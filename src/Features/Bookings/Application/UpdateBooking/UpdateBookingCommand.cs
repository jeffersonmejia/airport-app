namespace Airport.Features.Bookings.Application.UpdateBooking;

public sealed record UpdateBookingCommand(
    int BookingId,
    string? Seat,
    decimal Price,
    uint Version);
