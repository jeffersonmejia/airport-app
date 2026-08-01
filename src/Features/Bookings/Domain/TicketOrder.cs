namespace Airport.Features.Bookings.Domain;

public sealed record TicketOrder(
    Guid Id,
    string UserId,
    int FlightId,
    string FlightNumber,
    string OriginCode,
    string DestinationCode,
    DateTimeOffset Departure,
    string FareCode,
    string FareName,
    decimal Total,
    string CurrencyCode,
    string Status,
    string PassengerFirstName,
    string PassengerLastName,
    string PassportNumber,
    DateTimeOffset CreatedAt,
    string? TicketNumber = null,
    DateTimeOffset? PaidAt = null)
{
    public const string PendingPayment = "PENDING_PAYMENT";
    public const string Paid = "PAID";
}
