using Airport.Features.Payments.Application.CapturePayPalOrder;
using Airport.Features.Payments.Application.Ports;
using Airport.Features.Payments.Domain;

namespace Airport.UnitTests.Payments;

public sealed class CapturePayPalOrderHandlerTests
{
    [Fact]
    public async Task HandleAsyncCompletesMatchingCapture()
    {
        var ticketOrderId = Guid.NewGuid();
        var store = new StubStore(new RecordedPayPalPayment(
            Guid.NewGuid(), ticketOrderId, "user-1", 42, "ECONOMY", "PAYPAL-1", null,
            "create-key", "CREATED", 83m, "USD"));
        var handler = new CapturePayPalOrderHandler(
            new StubGateway(new PayPalCapture(
                "PAYPAL-1", "COMPLETED", "CAPTURE-1", PaymentMoney.Create(83m, "USD"))),
            store);

        var result = await handler.HandleAsync(
            new CapturePayPalOrderCommand("PAYPAL-1", "user-1", "capture-key"),
            CancellationToken.None);

        Assert.Equal(ticketOrderId, result.TicketOrderId);
        Assert.True(store.Completed);
    }

    [Fact]
    public async Task HandleAsyncRejectsDifferentAmount()
    {
        var store = new StubStore(new RecordedPayPalPayment(
            Guid.NewGuid(), Guid.NewGuid(), "user-1", 42, "ECONOMY", "PAYPAL-1", null,
            "create-key", "CREATED", 83m, "USD"));
        var handler = new CapturePayPalOrderHandler(
            new StubGateway(new PayPalCapture(
                "PAYPAL-1", "COMPLETED", "CAPTURE-1", PaymentMoney.Create(1m, "USD"))),
            store);

        await Assert.ThrowsAsync<PaymentOrderException>(() => handler.HandleAsync(
            new CapturePayPalOrderCommand("PAYPAL-1", "user-1", "capture-key"),
            CancellationToken.None));
    }

    private sealed class StubGateway(PayPalCapture capture) : IPayPalGateway
    {
        public Task<PayPalOrder> CreateOrderAsync(CreatePayPalOrderRequest request, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<PayPalCapture> CaptureOrderAsync(string orderId, string idempotencyKey, CancellationToken cancellationToken) => Task.FromResult(capture);
    }

    private sealed class StubStore(RecordedPayPalPayment payment) : IPaymentOrderStore
    {
        public bool Completed { get; private set; }
        public Task<PayableTicketOrder?> FindPayableAsync(Guid orderId, string userId, CancellationToken cancellationToken) => Task.FromResult<PayableTicketOrder?>(null);
        public Task<RecordedPayPalPayment?> FindByIdempotencyKeyAsync(string key, string userId, CancellationToken cancellationToken) => Task.FromResult<RecordedPayPalPayment?>(null);
        public Task<RecordedPayPalPayment?> FindByProviderOrderAsync(string providerOrderId, string userId, CancellationToken cancellationToken) => Task.FromResult<RecordedPayPalPayment?>(payment);
        public Task RecordCreatedAsync(PayableTicketOrder order, string providerOrderId, string? approvalUrl, string idempotencyKey, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task CompleteAsync(RecordedPayPalPayment value, string captureId, decimal capturedAmount, string currencyCode, CancellationToken cancellationToken) { Completed = true; return Task.CompletedTask; }
    }
}
