using Airport.Features.Payments.Application.Ports;
using Airport.Features.Payments.Domain;

namespace Airport.Features.Payments.Application.CreatePayPalOrder;

public sealed class CreatePayPalOrderHandler(IPayPalGateway gateway)
{
    public async Task<CreatePayPalOrderResponse> HandleAsync(
        CreatePayPalOrderCommand command,
        CancellationToken cancellationToken)
    {
        var request = new CreatePayPalOrderRequest(
            PaymentMoney.Create(command.Amount, command.CurrencyCode),
            command.ReferenceId.Trim(),
            command.Description.Trim(),
            command.IdempotencyKey.Trim());
        var order = await gateway.CreateOrderAsync(request, cancellationToken);

        return CreatePayPalOrderResponse.FromDomain(order);
    }
}
