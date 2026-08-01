namespace Airport.Features.Payments.Application.Ports;

public interface IPaymentOrderStore
{
    Task<PayableTicketOrder?> FindPayableAsync(Guid orderId, string userId, CancellationToken cancellationToken);
    Task<RecordedPayPalPayment?> FindByIdempotencyKeyAsync(string key, string userId, CancellationToken cancellationToken);
    Task<RecordedPayPalPayment?> FindByProviderOrderAsync(string providerOrderId, string userId, CancellationToken cancellationToken);
    Task RecordCreatedAsync(
        PayableTicketOrder order,
        string providerOrderId,
        string? approvalUrl,
        string idempotencyKey,
        CancellationToken cancellationToken);
    Task CompleteAsync(
        RecordedPayPalPayment payment,
        string captureId,
        decimal capturedAmount,
        string currencyCode,
        CancellationToken cancellationToken);
}

public sealed record PayableTicketOrder(
    Guid OrderId,
    string UserId,
    string FlightNumber,
    decimal Total,
    string CurrencyCode);

public sealed record RecordedPayPalPayment(
    Guid PaymentId,
    Guid OrderId,
    string UserId,
    int FlightId,
    string FareCode,
    string ProviderOrderId,
    string? ApprovalUrl,
    string IdempotencyKey,
    string Status,
    decimal Amount,
    string CurrencyCode);
