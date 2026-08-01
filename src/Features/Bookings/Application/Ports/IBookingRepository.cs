using Airport.Features.Bookings.Domain;

namespace Airport.Features.Bookings.Application.Ports;

public interface IBookingRepository
{
    Task<FlightOffer?> FindFlightOfferAsync(int flightId, CancellationToken cancellationToken);
    Task AddAsync(TicketOrder order, CancellationToken cancellationToken);
    Task<TicketOrder?> FindOwnedAsync(Guid orderId, string userId, CancellationToken cancellationToken);
    Task<BookingHistoryPage> SearchOwnedAsync(string userId, int page, int pageSize, CancellationToken cancellationToken);
}

public sealed record FlightOffer(
    int FlightId,
    string FlightNumber,
    string OriginCode,
    string DestinationCode,
    DateTimeOffset Departure,
    DateTimeOffset Arrival);

public sealed record BookingHistoryPage(IReadOnlyList<TicketOrder> Items, int TotalItems);
