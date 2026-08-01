using Airport.Features.Payments.Domain;

namespace Airport.Features.Payments.Application.CapturePayPalOrder;

public sealed record CapturePayPalOrderResponse(
    Guid TicketOrderId,
    string OrderId,
    string Status,
    string? CaptureId,
    decimal? Amount,
    string? CurrencyCode)
{
    public static CapturePayPalOrderResponse FromDomain(Guid ticketOrderId, PayPalCapture capture) => new(
        ticketOrderId,
        capture.OrderId,
        capture.Status,
        capture.CaptureId,
        capture.CapturedAmount?.Amount,
        capture.CapturedAmount?.CurrencyCode);
}
