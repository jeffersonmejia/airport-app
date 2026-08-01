namespace Airport.Features.Bookings.Infrastructure.Persistence;

public sealed class OrderRow
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public int FlightId { get; init; }
    public string FlightNumber { get; init; } = string.Empty;
    public string OriginCode { get; init; } = string.Empty;
    public string DestinationCode { get; init; } = string.Empty;
    public DateTimeOffset Departure { get; init; }
    public string FareCode { get; init; } = string.Empty;
    public string FareName { get; init; } = string.Empty;
    public decimal Total { get; init; }
    public string CurrencyCode { get; init; } = "USD";
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; set; }
    public OrderDetailRow Detail { get; init; } = default!;
    public PurchasedTicketRow? Ticket { get; set; }
    public ICollection<PaymentRow> Payments { get; init; } = [];
}

public sealed class OrderDetailRow
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public string PassengerFirstName { get; init; } = string.Empty;
    public string PassengerLastName { get; init; } = string.Empty;
    public string PassportNumber { get; init; } = string.Empty;
    public int Quantity { get; init; } = 1;
    public decimal UnitPrice { get; init; }
    public OrderRow Order { get; init; } = default!;
}

public sealed class PurchasedTicketRow
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public int FlightId { get; init; }
    public string TicketNumber { get; init; } = string.Empty;
    public string FareCode { get; init; } = string.Empty;
    public DateTimeOffset IssuedAt { get; init; }
    public OrderRow Order { get; init; } = default!;
}

public sealed class PaymentRow
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public string Provider { get; init; } = "PAYPAL";
    public string ProviderOrderId { get; init; } = string.Empty;
    public string? ApprovalUrl { get; init; }
    public string? ProviderCaptureId { get; set; }
    public string IdempotencyKey { get; init; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; init; }
    public string CurrencyCode { get; init; } = "USD";
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? CompletedAt { get; set; }
    public OrderRow Order { get; init; } = default!;
}

internal sealed class FlightOfferRow
{
    public int FlightId { get; init; }
    public string FlightNumber { get; init; } = string.Empty;
    public int OriginAirportId { get; init; }
    public int DestinationAirportId { get; init; }
    public DateTimeOffset Departure { get; init; }
    public DateTimeOffset Arrival { get; init; }
}

internal sealed class BookingAirportRow
{
    public int AirportId { get; init; }
    public string? Iata { get; init; }
    public string Icao { get; init; } = string.Empty;
}
