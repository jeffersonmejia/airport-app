namespace Airport.Features.Administration.Infrastructure.Persistence;

public sealed class CommerceOrderRow
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string FlightNumber { get; init; } = string.Empty;
    public string OriginCode { get; init; } = string.Empty;
    public string DestinationCode { get; init; } = string.Empty;
    public decimal Total { get; init; }
    public string CurrencyCode { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
}

public sealed class CommercePaymentRow
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public string ProviderOrderId { get; init; } = string.Empty;
    public string? ProviderCaptureId { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}

public sealed class CommerceTicketRow
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public string TicketNumber { get; init; } = string.Empty;
}
