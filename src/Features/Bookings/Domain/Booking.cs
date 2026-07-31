namespace Airport.Features.Bookings.Domain;

public sealed record Booking(
    int BookingId,
    int FlightId,
    int PassengerId,
    string? Seat,
    decimal Price,
    DateTimeOffset Departure,
    bool IsCancelled,
    uint Version)
{
    public bool CanModify(DateTimeOffset now) => !IsCancelled && Departure > now;
}
