using Airport.Features.Payments.Application.Ports;

namespace Airport.Features.Payments.Application.CapturePayPalOrder;

public sealed class CapturePayPalOrderHandler(IPayPalGateway gateway)
{
    public async Task<CapturePayPalOrderResponse> HandleAsync(
        CapturePayPalOrderCommand command,
        CancellationToken cancellationToken)
    {
        var capture = await gateway.CaptureOrderAsync(
            command.OrderId.Trim(),
            command.IdempotencyKey.Trim(),
            cancellationToken);

        return CapturePayPalOrderResponse.FromDomain(capture);
    }
}
