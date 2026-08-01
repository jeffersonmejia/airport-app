using Airport.Features.Payments.Domain;

namespace Airport.Features.Payments.Application.Ports;

public interface IPayPalGateway
{
    Task<PayPalOrder> CreateOrderAsync(
        CreatePayPalOrderRequest request,
        CancellationToken cancellationToken);

    Task<PayPalCapture> CaptureOrderAsync(
        string orderId,
        string idempotencyKey,
        CancellationToken cancellationToken);
}

public sealed record CreatePayPalOrderRequest(
    PaymentMoney Amount,
    string ReferenceId,
    string Description,
    string IdempotencyKey);
