namespace Airport.Features.Payments.Presentation.Web.Models;

public sealed record CreatePayPalOrderInput(
    Guid OrderId);

public sealed record PayPalOrderViewModel(
    string OrderId,
    string Status,
    string? ApprovalUrl);

public sealed record PayPalCaptureViewModel(
    Guid TicketOrderId,
    string OrderId,
    string Status,
    string? CaptureId,
    decimal? Amount,
    string? CurrencyCode);
