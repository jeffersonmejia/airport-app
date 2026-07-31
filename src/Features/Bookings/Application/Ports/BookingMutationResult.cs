using Airport.Features.Bookings.Domain;

namespace Airport.Features.Bookings.Application.Ports;

public enum BookingMutationStatus
{
    Success,
    NotFound,
    RelatedResourceNotFound,
    HistoricalFlight,
    AlreadyCancelled,
    SeatOccupied,
    ConcurrencyConflict
}

public sealed record BookingMutationResult(
    BookingMutationStatus Status,
    Booking? Booking = null);
