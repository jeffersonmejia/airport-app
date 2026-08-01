namespace Airport.Features.Administration.Application.GetCommerceOverview;

public sealed class GetCommerceOverviewHandler(ICommerceOverviewReader reader)
{
    public Task<CommerceOverviewResponse> HandleAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken) => reader.ReadAsync(page, pageSize, cancellationToken);
}

public sealed record CommerceOrderItem(
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

public sealed record CommerceOverviewResponse(
    IReadOnlyCollection<CommerceOrderItem> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    int PaidOrders,
    decimal CapturedTotal);
