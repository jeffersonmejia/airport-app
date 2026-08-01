using Airport.Features.Bookings.Application.Ports;
using Airport.Features.Bookings.Domain;
using Microsoft.EntityFrameworkCore;

namespace Airport.Features.Bookings.Infrastructure.Persistence;

internal sealed class PostgresBookingRepository(BookingsDbContext dbContext) : IBookingRepository
{
    public async Task<FlightOffer?> FindFlightOfferAsync(int flightId, CancellationToken cancellationToken) =>
        await (from flight in dbContext.Flights.AsNoTracking()
               join origin in dbContext.Airports.AsNoTracking() on flight.OriginAirportId equals origin.AirportId
               join destination in dbContext.Airports.AsNoTracking() on flight.DestinationAirportId equals destination.AirportId
               where flight.FlightId == flightId
               select new FlightOffer(
                   flight.FlightId,
                   flight.FlightNumber,
                   (origin.Iata ?? origin.Icao).Trim(),
                   (destination.Iata ?? destination.Icao).Trim(),
                   flight.Departure,
                   flight.Arrival))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task AddAsync(TicketOrder order, CancellationToken cancellationToken)
    {
        var row = new OrderRow
        {
            Id = order.Id,
            UserId = order.UserId,
            FlightId = order.FlightId,
            FlightNumber = order.FlightNumber,
            OriginCode = order.OriginCode,
            DestinationCode = order.DestinationCode,
            Departure = order.Departure,
            FareCode = order.FareCode,
            FareName = order.FareName,
            Total = order.Total,
            CurrencyCode = order.CurrencyCode,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.CreatedAt,
            Detail = new OrderDetailRow
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                PassengerFirstName = order.PassengerFirstName,
                PassengerLastName = order.PassengerLastName,
                PassportNumber = order.PassportNumber,
                UnitPrice = order.Total
            }
        };
        dbContext.Orders.Add(row);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<TicketOrder?> FindOwnedAsync(Guid orderId, string userId, CancellationToken cancellationToken)
    {
        var row = await dbContext.Orders.AsNoTracking()
            .Include(order => order.Detail)
            .Include(order => order.Ticket)
            .SingleOrDefaultAsync(order => order.Id == orderId && order.UserId == userId, cancellationToken);
        return row is null ? null : Map(row);
    }

    public async Task<BookingHistoryPage> SearchOwnedAsync(
        string userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Orders.AsNoTracking().Where(order => order.UserId == userId);
        var total = await query.CountAsync(cancellationToken);
        var rows = await query.Include(order => order.Detail).Include(order => order.Ticket)
            .OrderByDescending(order => order.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return new BookingHistoryPage(rows.Select(Map).ToArray(), total);
    }

    private static TicketOrder Map(OrderRow row) => new(
        row.Id,
        row.UserId,
        row.FlightId,
        row.FlightNumber,
        row.OriginCode,
        row.DestinationCode,
        row.Departure,
        row.FareCode,
        row.FareName,
        row.Total,
        row.CurrencyCode,
        row.Status,
        row.Detail.PassengerFirstName,
        row.Detail.PassengerLastName,
        row.Detail.PassportNumber,
        row.CreatedAt,
        row.Ticket?.TicketNumber,
        row.Ticket?.IssuedAt);
}
