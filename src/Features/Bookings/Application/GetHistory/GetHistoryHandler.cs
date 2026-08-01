using Airport.Features.Bookings.Application.Ports;

namespace Airport.Features.Bookings.Application.GetHistory;

public sealed class GetHistoryHandler(IBookingRepository repository)
{
    public async Task<BookingHistoryResponse> HandleAsync(
        string userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var result = await repository.SearchOwnedAsync(userId, page, pageSize, cancellationToken);
        return new BookingHistoryResponse(
            result.Items.Select(order => new BookingHistoryItem(
                order.Id,
                order.FlightNumber,
                order.OriginCode,
                order.DestinationCode,
                order.Departure,
                order.FareName,
                order.Total,
                order.CurrencyCode,
                order.Status,
                order.TicketNumber,
                order.CreatedAt)).ToArray(),
            page,
            pageSize,
            result.TotalItems,
            result.TotalItems == 0 ? 0 : (int)Math.Ceiling(result.TotalItems / (double)pageSize));
    }
}

public sealed record BookingHistoryItem(
    Guid OrderId,
    string FlightNumber,
    string OriginCode,
    string DestinationCode,
    DateTimeOffset Departure,
    string FareName,
    decimal Total,
    string CurrencyCode,
    string Status,
    string? TicketNumber,
    DateTimeOffset CreatedAt);

public sealed record BookingHistoryResponse(
    IReadOnlyCollection<BookingHistoryItem> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);
