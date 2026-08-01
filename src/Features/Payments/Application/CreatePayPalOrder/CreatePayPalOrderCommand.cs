namespace Airport.Features.Payments.Application.CreatePayPalOrder;

public sealed record CreatePayPalOrderCommand(
    decimal Amount,
    string CurrencyCode,
    string ReferenceId,
    string Description,
    string IdempotencyKey);
