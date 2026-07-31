using Airport.Features.Bookings.Domain;

namespace Airport.Features.Bookings.Application;

public sealed record BookingResponse(
    int BookingId,
    int FlightId,
    int PassengerId,
    string? Seat,
    decimal Price,
    DateTimeOffset Departure,
    bool IsCancelled,
    bool CanModify,
    uint Version)
{
    public static BookingResponse FromDomain(Booking booking, DateTimeOffset now) => new(
        booking.BookingId,
        booking.FlightId,
        booking.PassengerId,
        booking.Seat,
        booking.Price,
        booking.Departure,
        booking.IsCancelled,
        booking.CanModify(now),
        booking.Version);
}
