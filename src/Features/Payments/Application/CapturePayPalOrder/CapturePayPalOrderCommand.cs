namespace Airport.Features.Payments.Application.CapturePayPalOrder;

public sealed record CapturePayPalOrderCommand(
    string OrderId,
    string IdempotencyKey);
