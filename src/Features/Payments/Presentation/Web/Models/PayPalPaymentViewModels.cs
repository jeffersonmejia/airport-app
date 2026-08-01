namespace Airport.Features.Payments.Presentation.Web.Models;

public sealed record CreatePayPalOrderInput(
    decimal Amount,
    string CurrencyCode,
    string ReferenceId,
    string Description);

public sealed record PayPalOrderViewModel(
    string OrderId,
    string Status,
    string? ApprovalUrl);

public sealed record PayPalCaptureViewModel(
    string OrderId,
    string Status,
    string? CaptureId,
    decimal? Amount,
    string? CurrencyCode);
