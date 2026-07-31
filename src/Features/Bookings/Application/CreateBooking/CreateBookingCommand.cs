namespace Airport.Features.Bookings.Application.CreateBooking;

public sealed record CreateBookingCommand(
    int FlightId,
    int PassengerId,
    string? Seat,
    decimal Price);
