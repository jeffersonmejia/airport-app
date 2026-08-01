using Airport.Features.Administration.Application.GetCommerceOverview;
using Microsoft.EntityFrameworkCore;

namespace Airport.Features.Administration.Infrastructure.Persistence;

internal sealed class PostgresCommerceOverviewReader(AdministrationDbContext dbContext)
    : ICommerceOverviewReader
{
    public async Task<CommerceOverviewResponse> ReadAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var total = await dbContext.Orders.CountAsync(cancellationToken);
        var paidOrders = await dbContext.Orders.CountAsync(order => order.Status == "PAID", cancellationToken);
        var capturedTotal = await dbContext.Payments
            .Where(payment => payment.Status == "COMPLETED")
            .SumAsync(payment => (decimal?)payment.Amount, cancellationToken) ?? 0m;
        var items = await (
            from order in dbContext.Orders.AsNoTracking()
            join payment in dbContext.Payments.AsNoTracking() on order.Id equals payment.OrderId into payments
            from payment in payments.OrderByDescending(value => value.Id).Take(1).DefaultIfEmpty()
            join ticket in dbContext.Tickets.AsNoTracking() on order.Id equals ticket.OrderId into tickets
            from ticket in tickets.DefaultIfEmpty()
            orderby order.CreatedAt descending
            select new CommerceOrderItem(
                order.Id,
                order.UserId,
                order.FlightNumber,
                order.OriginCode + " → " + order.DestinationCode,
                order.Total,
                order.CurrencyCode,
                order.Status,
                payment == null ? null : payment.Status,
                payment == null ? null : payment.ProviderOrderId,
                payment == null ? null : payment.ProviderCaptureId,
                ticket == null ? null : ticket.TicketNumber,
                order.CreatedAt))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return new CommerceOverviewResponse(
            items,
            page,
            pageSize,
            total,
            total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize),
            paidOrders,
            capturedTotal);
    }
}
