namespace Airport.Features.Bookings.Infrastructure.Persistence;

public sealed class BookingRow
{
    public int BookingId { get; init; }
    public int FlightId { get; init; }
    public string? Seat { get; set; }
    public int PassengerId { get; init; }
    public decimal Price { get; set; }
    public uint Version { get; init; }
}
