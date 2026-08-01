using System.ComponentModel.DataAnnotations;

namespace Airport.Features.Bookings.Presentation.Web.Models;

public sealed class CheckoutInput
{
    [Required, MinLength(2), MaxLength(100)]
    public string PassengerFirstName { get; set; } = string.Empty;
    [Required, MinLength(2), MaxLength(100)]
    public string PassengerLastName { get; set; } = string.Empty;
    [Required, MinLength(6), MaxLength(20)]
    public string PassportNumber { get; set; } = string.Empty;
}

public sealed record OrderViewModel(
    Guid OrderId,
    string Status,
    decimal Total,
    string CurrencyCode,
    string FlightNumber,
    string Route,
    DateTimeOffset Departure,
    string FareName);

public sealed record BookingHistoryItemViewModel(
    Guid OrderId,
    string FlightNumber,
    string OriginCode,
    string DestinationCode,
    DateTimeOffset Departure,
    string FareName,
    decimal Total,
    string CurrencyCode,
    string Status,
    string? TicketNumber,
    DateTimeOffset CreatedAt);

public sealed record BookingHistoryViewModel(
    IReadOnlyCollection<BookingHistoryItemViewModel> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);

public sealed record ReceiptViewModel(
    Guid OrderId,
    string? TicketNumber,
    string Status,
    string PassengerFirstName,
    string PassengerLastName,
    string PassportNumber,
    string FlightNumber,
    string OriginCode,
    string DestinationCode,
    DateTimeOffset Departure,
    string FareName,
    decimal Total,
    string CurrencyCode,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PaidAt);
