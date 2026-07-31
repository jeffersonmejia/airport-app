using Airport.Features.Bookings.Domain;

namespace Airport.Features.Bookings.Application.Ports;

public sealed record BookingPage(
    IReadOnlyCollection<Booking> Items,
    int Page,
    bool HasNextPage,
    int TotalItems,
    bool TotalApproximate);
