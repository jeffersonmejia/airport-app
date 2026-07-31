namespace Airport.Features.Bookings.Presentation.Web.Models;

public sealed record BookingViewModel(
    int BookingId,
    int FlightId,
    int PassengerId,
    string? Seat,
    decimal Price,
    DateTimeOffset Departure,
    bool IsCancelled,
    bool CanModify,
    uint Version);
