namespace Airport.Features.Administration.Presentation.Web.Models;

public sealed record DatabaseSummaryViewModel(
    long Total,
    bool IsApproximate,
    DateTimeOffset UpdatedAt,
    IReadOnlyCollection<DatabaseSummaryItemViewModel> Items);

public sealed record DatabaseSummaryItemViewModel(string Label, long Count);

public sealed record CommerceOrderViewModel(
    Guid OrderId,
    string UserId,
    string FlightNumber,
    string Route,
    decimal Total,
    string CurrencyCode,
    string OrderStatus,
    string? PaymentStatus,
    string? ProviderOrderId,
    string? ProviderCaptureId,
    string? TicketNumber,
    DateTimeOffset CreatedAt);

public sealed record CommerceOverviewViewModel(
    IReadOnlyCollection<CommerceOrderViewModel> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    int PaidOrders,
    decimal CapturedTotal);
