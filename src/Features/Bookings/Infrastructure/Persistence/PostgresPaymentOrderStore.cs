using System.Data;
using System.Linq.Expressions;
using Airport.Features.Bookings.Domain;
using Airport.Features.Payments.Application.Ports;
using Microsoft.EntityFrameworkCore;

namespace Airport.Features.Bookings.Infrastructure.Persistence;

internal sealed class PostgresPaymentOrderStore(
    BookingsDbContext dbContext,
    TimeProvider timeProvider) : IPaymentOrderStore
{
    public Task<PayableTicketOrder?> FindPayableAsync(
        Guid orderId,
        string userId,
        CancellationToken cancellationToken) => dbContext.Orders
        .AsNoTracking()
        .Where(order => order.Id == orderId &&
                        order.UserId == userId &&
                        order.Status == TicketOrder.PendingPayment)
        .Select(order => new PayableTicketOrder(
            order.Id,
            order.UserId,
            order.FlightNumber,
            order.Total,
            order.CurrencyCode))
        .SingleOrDefaultAsync(cancellationToken);

    public Task<RecordedPayPalPayment?> FindByIdempotencyKeyAsync(
        string key,
        string userId,
        CancellationToken cancellationToken) => QueryPayment(
            userId,
            payment => payment.IdempotencyKey == key,
            cancellationToken);

    public Task<RecordedPayPalPayment?> FindByProviderOrderAsync(
        string providerOrderId,
        string userId,
        CancellationToken cancellationToken) => QueryPayment(
            userId,
            payment => payment.ProviderOrderId == providerOrderId,
            cancellationToken);

    public async Task RecordCreatedAsync(
        PayableTicketOrder order,
        string providerOrderId,
        string? approvalUrl,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        dbContext.Payments.Add(new PaymentRow
        {
            Id = Guid.NewGuid(),
            OrderId = order.OrderId,
            ProviderOrderId = providerOrderId,
            ApprovalUrl = approvalUrl,
            IdempotencyKey = idempotencyKey,
            Status = "CREATED",
            Amount = order.Total,
            CurrencyCode = order.CurrencyCode,
            CreatedAt = timeProvider.GetUtcNow()
        });
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CompleteAsync(
        RecordedPayPalPayment payment,
        string captureId,
        decimal capturedAmount,
        string currencyCode,
        CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);
        var row = await dbContext.Payments
            .Include(value => value.Order)
            .ThenInclude(order => order.Ticket)
            .SingleAsync(value => value.Id == payment.PaymentId, cancellationToken);
        if (row.Status == "COMPLETED")
        {
            await transaction.CommitAsync(cancellationToken);
            return;
        }
        if (row.Amount != capturedAmount ||
            !string.Equals(row.CurrencyCode, currencyCode, StringComparison.OrdinalIgnoreCase))
        {
            throw new PaymentOrderException("El monto capturado no corresponde a la orden.");
        }

        var now = timeProvider.GetUtcNow();
        row.ProviderCaptureId = captureId;
        row.Status = "COMPLETED";
        row.CompletedAt = now;
        row.Order.Status = TicketOrder.Paid;
        row.Order.UpdatedAt = now;
        row.Order.Ticket ??= dbContext.PurchasedTickets.Add(new PurchasedTicketRow
        {
            Id = Guid.NewGuid(),
            OrderId = row.OrderId,
            FlightId = row.Order.FlightId,
            FareCode = row.Order.FareCode,
            TicketNumber = $"APT-{now:yyyyMMdd}-{Guid.NewGuid():N}"[..21].ToUpperInvariant(),
            IssuedAt = now
        }).Entity;
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private Task<RecordedPayPalPayment?> QueryPayment(
        string userId,
        Expression<Func<PaymentRow, bool>> filter,
        CancellationToken cancellationToken) => dbContext.Payments
        .AsNoTracking()
        .Where(payment => payment.Order.UserId == userId)
        .Where(filter)
        .Select(payment => new RecordedPayPalPayment(
            payment.Id,
            payment.OrderId,
            payment.Order.UserId,
            payment.Order.FlightId,
            payment.Order.FareCode,
            payment.ProviderOrderId,
            payment.ApprovalUrl,
            payment.IdempotencyKey,
            payment.Status,
            payment.Amount,
            payment.CurrencyCode))
        .SingleOrDefaultAsync(cancellationToken);
}
