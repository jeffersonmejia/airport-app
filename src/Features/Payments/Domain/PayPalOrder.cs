namespace Airport.Features.Payments.Domain;

public sealed record PayPalOrder(
    string OrderId,
    string Status,
    Uri? ApprovalUrl);
