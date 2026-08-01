using Airport.Features.Bookings.Application.CreateOrder;
using Airport.Features.Bookings.Application.Ports;

namespace Airport.Features.Bookings.Application.GetOrder;

public sealed class GetOrderHandler(IBookingRepository repository)
{
    public async Task<CreateOrderResponse?> HandleAsync(
        Guid orderId,
        string userId,
        CancellationToken cancellationToken)
    {
        var order = await repository.FindOwnedAsync(orderId, userId, cancellationToken);
        return order is null ? null : CreateOrderResponse.FromDomain(order);
    }
}
