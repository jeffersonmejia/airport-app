using Airport.Features.Payments.Application.Ports;
using Airport.Features.Payments.Domain;

namespace Airport.Features.Payments.Application.CreatePayPalOrder;

public sealed class CreatePayPalOrderHandler(IPayPalGateway gateway, IPaymentOrderStore orderStore)
{
    public async Task<CreatePayPalOrderResponse> HandleAsync(
        CreatePayPalOrderCommand command,
        CancellationToken cancellationToken)
    {
        var idempotencyKey = command.IdempotencyKey.Trim();
        var existing = await orderStore.FindByIdempotencyKeyAsync(
            idempotencyKey,
            command.UserId,
            cancellationToken);
        if (existing is not null)
        {
            if (existing.OrderId != command.TicketOrderId)
            {
                throw new PaymentOrderException(
                    "El identificador de idempotencia ya pertenece a otra orden.");
            }

            return new CreatePayPalOrderResponse(
                existing.ProviderOrderId,
                existing.Status,
                existing.ApprovalUrl);
        }

        var ticketOrder = await orderStore.FindPayableAsync(
            command.TicketOrderId,
            command.UserId,
            cancellationToken) ?? throw new PaymentOrderException(
                "La orden no existe, no pertenece al usuario o ya fue pagada.");
        var request = new CreatePayPalOrderRequest(
            PaymentMoney.Create(ticketOrder.Total, ticketOrder.CurrencyCode),
            ticketOrder.OrderId.ToString("N"),
            $"Boleto {ticketOrder.FlightNumber}",
            idempotencyKey);
        var order = await gateway.CreateOrderAsync(request, cancellationToken);
        await orderStore.RecordCreatedAsync(
            ticketOrder,
            order.OrderId,
            order.ApprovalUrl?.AbsoluteUri,
            command.IdempotencyKey.Trim(),
            cancellationToken);

        return CreatePayPalOrderResponse.FromDomain(order);
    }
}
