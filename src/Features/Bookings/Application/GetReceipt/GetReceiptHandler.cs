using Airport.Features.Bookings.Application.Ports;

namespace Airport.Features.Bookings.Application.GetReceipt;

public sealed class GetReceiptHandler(IBookingRepository repository)
{
    public async Task<ReceiptResponse?> HandleAsync(
        Guid orderId,
        string userId,
        CancellationToken cancellationToken)
    {
        var order = await repository.FindOwnedAsync(orderId, userId, cancellationToken);
        return order is null ? null : new ReceiptResponse(
            order.Id,
            order.TicketNumber,
            order.Status,
            order.PassengerFirstName,
            order.PassengerLastName,
            order.PassportNumber,
            order.FlightNumber,
            order.OriginCode,
            order.DestinationCode,
            order.Departure,
            order.FareName,
            order.Total,
            order.CurrencyCode,
            order.CreatedAt,
            order.PaidAt);
    }
}

public sealed record ReceiptResponse(
    Guid OrderId,
    string? TicketNumber,
    string Status,
    string PassengerFirstName,
    string PassengerLastName,
    string PassportNumber,
    string FlightNumber,
    string OriginCode,
    string DestinationCode,
    DateTimeOffset Departure,
    string FareName,
    decimal Total,
    string CurrencyCode,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PaidAt);
