namespace Airport.Features.Bookings.Infrastructure.Persistence;

public sealed class BookingCancellationRow
{
    public int BookingId { get; init; }
    public DateTimeOffset CancelledAt { get; init; }
    public int CancelledBy { get; init; }
    public string Reason { get; init; } = string.Empty;
}
