namespace Airport.Features.Bookings.Infrastructure.Persistence;

public sealed class BookingFlightRow
{
    public int FlightId { get; init; }
    public DateTimeOffset Departure { get; init; }
}
