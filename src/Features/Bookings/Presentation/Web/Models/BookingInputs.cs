namespace Airport.Features.Bookings.Presentation.Web.Models;

public sealed class CreateBookingInput
{
    public int? FlightId { get; set; }
    public int? PassengerId { get; set; }
    public string? Seat { get; set; }
    public decimal? Price { get; set; }
}

public sealed class UpdateBookingInput
{
    public string? Seat { get; set; }
    public decimal Price { get; set; }
    public uint Version { get; set; }
}
