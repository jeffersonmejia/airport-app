using Airport.Features.Bookings.Application.CreateOrder;
using Airport.Features.Bookings.Application.Ports;
using Airport.Features.Bookings.Domain;

namespace Airport.UnitTests.Bookings;

public sealed class CreateOrderHandlerTests
{
    [Fact]
    public async Task HandleAsyncUsesAuthoritativeFlightAndFare()
    {
        var now = new DateTimeOffset(2026, 8, 1, 10, 0, 0, TimeSpan.Zero);
        var repository = new StubRepository(new FlightOffer(
            42,
            "AE042",
            "UIO",
            "GYE",
            now.AddDays(1),
            now.AddDays(1).AddHours(1)));
        var handler = new CreateOrderHandler(repository, new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreateOrderCommand("user-1", 42, "ECONOMY", "Ana", "Pérez", "AB123456"),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(83m, result.Total);
        Assert.Equal("USD", result.CurrencyCode);
        Assert.Equal(TicketOrder.PendingPayment, result.Status);
        Assert.NotNull(repository.Added);
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }

    private sealed class StubRepository(FlightOffer? offer) : IBookingRepository
    {
        public TicketOrder? Added { get; private set; }
        public Task<FlightOffer?> FindFlightOfferAsync(int flightId, CancellationToken cancellationToken) => Task.FromResult(offer);
        public Task AddAsync(TicketOrder order, CancellationToken cancellationToken) { Added = order; return Task.CompletedTask; }
        public Task<TicketOrder?> FindOwnedAsync(Guid orderId, string userId, CancellationToken cancellationToken) => Task.FromResult<TicketOrder?>(null);
        public Task<BookingHistoryPage> SearchOwnedAsync(string userId, int page, int pageSize, CancellationToken cancellationToken) => Task.FromResult(new BookingHistoryPage([], 0));
    }
}
