using Airport.Features.Payments.Domain;

namespace Airport.Features.Payments.Application.CapturePayPalOrder;

public sealed record CapturePayPalOrderResponse(
    string OrderId,
    string Status,
    string? CaptureId,
    decimal? Amount,
    string? CurrencyCode)
{
    public static CapturePayPalOrderResponse FromDomain(PayPalCapture capture) => new(
        capture.OrderId,
        capture.Status,
        capture.CaptureId,
        capture.CapturedAmount?.Amount,
        capture.CapturedAmount?.CurrencyCode);
}
