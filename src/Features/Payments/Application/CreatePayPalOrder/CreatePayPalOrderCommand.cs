namespace Airport.Features.Payments.Application.CreatePayPalOrder;

public sealed record CreatePayPalOrderCommand(
    Guid TicketOrderId,
    string UserId,
    string IdempotencyKey);
