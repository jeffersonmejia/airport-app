namespace Airport.Features.Payments.Domain;

public sealed record PayPalCapture(
    string OrderId,
    string Status,
    string? CaptureId,
    PaymentMoney? CapturedAmount);
