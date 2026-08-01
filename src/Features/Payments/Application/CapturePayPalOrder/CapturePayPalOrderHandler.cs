using Airport.Features.Payments.Application.Ports;

namespace Airport.Features.Payments.Application.CapturePayPalOrder;

public sealed class CapturePayPalOrderHandler(IPayPalGateway gateway, IPaymentOrderStore orderStore)
{
    public async Task<CapturePayPalOrderResponse> HandleAsync(
        CapturePayPalOrderCommand command,
        CancellationToken cancellationToken)
    {
        var payment = await orderStore.FindByProviderOrderAsync(
            command.OrderId.Trim(),
            command.UserId,
            cancellationToken) ?? throw new PaymentOrderException(
                "La orden PayPal no pertenece al usuario o no está registrada.");
        if (string.Equals(payment.Status, "COMPLETED", StringComparison.Ordinal))
        {
            throw new PaymentOrderException("La orden ya fue capturada.");
        }

        var capture = await gateway.CaptureOrderAsync(
            command.OrderId.Trim(),
            command.IdempotencyKey.Trim(),
            cancellationToken);
        var capturedAmount = capture.CapturedAmount;

        if (!string.Equals(capture.Status, "COMPLETED", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(capture.CaptureId) ||
            capturedAmount is null ||
            capturedAmount.Value.Amount != payment.Amount ||
            !string.Equals(
                capturedAmount.Value.CurrencyCode,
                payment.CurrencyCode,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new PaymentOrderException(
                "La captura de PayPal no coincide con el monto y la moneda de la orden.");
        }

        await orderStore.CompleteAsync(
            payment,
            capture.CaptureId,
            capturedAmount.Value.Amount,
            capturedAmount.Value.CurrencyCode,
            cancellationToken);

        return CapturePayPalOrderResponse.FromDomain(payment.OrderId, capture);
    }
}
