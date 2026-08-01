using Airport.Features.Payments.Domain;

namespace Airport.Features.Payments.Application.CreatePayPalOrder;

public sealed record CreatePayPalOrderResponse(
    string OrderId,
    string Status,
    string? ApprovalUrl)
{
    public static CreatePayPalOrderResponse FromDomain(PayPalOrder order) => new(
        order.OrderId,
        order.Status,
        order.ApprovalUrl?.AbsoluteUri);
}
