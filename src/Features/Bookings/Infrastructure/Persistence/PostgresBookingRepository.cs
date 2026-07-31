using Airport.Features.Bookings.Application.Ports;
using Airport.Features.Bookings.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Airport.Features.Bookings.Infrastructure.Persistence;

public sealed class PostgresBookingRepository(
    BookingsDbContext dbContext,
    TimeProvider timeProvider) : IBookingRepository
{
    private const string BookingEstimateSql = """
        SELECT GREATEST(c.reltuples, 0)::bigint AS "EstimatedRows"
        FROM pg_class AS c
        INNER JOIN pg_namespace AS n ON n.oid = c.relnamespace
        WHERE n.nspname = 'airportdb'
          AND c.relname = 'booking'
          AND c.relkind = 'r'
        """;

    public async Task<BookingPage> SearchAsync(
        int? bookingId,
        int? flightId,
        int? passengerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var bookings = dbContext.Bookings.AsNoTracking();
        if (bookingId is not null) bookings = bookings.Where(row => row.BookingId == bookingId);
        if (flightId is not null) bookings = bookings.Where(row => row.FlightId == flightId);
        if (passengerId is not null) bookings = bookings.Where(row => row.PassengerId == passengerId);

        var filtered = bookingId is not null || flightId is not null || passengerId is not null;
        var (totalItems, totalApproximate) = filtered
            ? (await bookings.CountAsync(cancellationToken), false)
            : (await EstimateTotalAsync(cancellationToken), true);

        var pageQuery = bookings
            .OrderByDescending(booking => booking.BookingId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize + 1);
        var rows = await BuildProjection(pageQuery).ToListAsync(cancellationToken);
        rows.Sort((left, right) => right.BookingId.CompareTo(left.BookingId));
        var hasNextPage = rows.Count > pageSize;

        return new BookingPage(
            rows.Take(pageSize).Select(ToDomain).ToArray(),
            page,
            hasNextPage,
            totalItems,
            totalApproximate);
    }

    private async Task<int> EstimateTotalAsync(CancellationToken cancellationToken)
    {
        var estimate = await dbContext.Database
            .SqlQueryRaw<BookingTableEstimate>(BookingEstimateSql)
            .FirstOrDefaultAsync(cancellationToken);
        return (int)Math.Min(estimate?.EstimatedRows ?? 0, int.MaxValue);
    }

    public async Task<Booking?> FindByIdAsync(
        int bookingId,
        CancellationToken cancellationToken)
    {
        var row = await BuildProjection(
                dbContext.Bookings.AsNoTracking().Where(item => item.BookingId == bookingId))
            .SingleOrDefaultAsync(cancellationToken);
        return row is null ? null : ToDomain(row);
    }

    public async Task<BookingMutationResult> CreateAsync(
        int flightId,
        int passengerId,
        string? seat,
        decimal price,
        CancellationToken cancellationToken)
    {
        var departure = await dbContext.Flights.AsNoTracking()
            .Where(row => row.FlightId == flightId)
            .Select(row => (DateTimeOffset?)row.Departure)
            .SingleOrDefaultAsync(cancellationToken);
        var passengerExists = await dbContext.Passengers.AsNoTracking()
            .AnyAsync(row => row.PassengerId == passengerId, cancellationToken);

        if (departure is null || !passengerExists)
        {
            return new BookingMutationResult(BookingMutationStatus.RelatedResourceNotFound);
        }

        if (departure <= timeProvider.GetUtcNow())
        {
            return new BookingMutationResult(BookingMutationStatus.HistoricalFlight);
        }

        var row = new BookingRow
        {
            FlightId = flightId,
            PassengerId = passengerId,
            Seat = seat,
            Price = price
        };
        dbContext.Bookings.Add(row);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return new BookingMutationResult(BookingMutationStatus.SeatOccupied);
        }

        return new BookingMutationResult(
            BookingMutationStatus.Success,
            ToDomain(row, departure.Value, false));
    }

    public async Task<BookingMutationResult> UpdateAsync(
        int bookingId,
        string? seat,
        decimal price,
        uint version,
        CancellationToken cancellationToken)
    {
        var row = await dbContext.Bookings.SingleOrDefaultAsync(
            booking => booking.BookingId == bookingId,
            cancellationToken);
        if (row is null) return new BookingMutationResult(BookingMutationStatus.NotFound);

        var departure = await dbContext.Flights.AsNoTracking()
            .Where(flight => flight.FlightId == row.FlightId)
            .Select(flight => flight.Departure)
            .SingleAsync(cancellationToken);
        var cancelled = await dbContext.Cancellations.AsNoTracking()
            .AnyAsync(item => item.BookingId == bookingId, cancellationToken);

        if (cancelled) return new BookingMutationResult(BookingMutationStatus.AlreadyCancelled);
        if (departure <= timeProvider.GetUtcNow())
        {
            return new BookingMutationResult(BookingMutationStatus.HistoricalFlight);
        }
        if (row.Version != version)
        {
            return new BookingMutationResult(BookingMutationStatus.ConcurrencyConflict);
        }

        row.Seat = seat;
        row.Price = price;

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new BookingMutationResult(BookingMutationStatus.ConcurrencyConflict);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return new BookingMutationResult(BookingMutationStatus.SeatOccupied);
        }

        return new BookingMutationResult(
            BookingMutationStatus.Success,
            ToDomain(row, departure, false));
    }

    public async Task<BookingMutationResult> CancelAsync(
        int bookingId,
        int employeeId,
        string reason,
        CancellationToken cancellationToken)
    {
        var booking = await FindByIdAsync(bookingId, cancellationToken);
        if (booking is null) return new BookingMutationResult(BookingMutationStatus.NotFound);
        if (booking.IsCancelled)
        {
            return new BookingMutationResult(BookingMutationStatus.AlreadyCancelled);
        }
        if (booking.Departure <= timeProvider.GetUtcNow())
        {
            return new BookingMutationResult(BookingMutationStatus.HistoricalFlight);
        }

        dbContext.Cancellations.Add(new BookingCancellationRow
        {
            BookingId = bookingId,
            CancelledAt = timeProvider.GetUtcNow(),
            CancelledBy = employeeId,
            Reason = reason
        });

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return new BookingMutationResult(BookingMutationStatus.AlreadyCancelled);
        }

        return new BookingMutationResult(
            BookingMutationStatus.Success,
            booking with { IsCancelled = true });
    }

    private IQueryable<BookingProjection> BuildProjection(IQueryable<BookingRow> bookings) =>
        from booking in bookings
        join flight in dbContext.Flights.AsNoTracking()
            on booking.FlightId equals flight.FlightId
        join cancellation in dbContext.Cancellations.AsNoTracking()
            on booking.BookingId equals cancellation.BookingId into cancellations
        from cancellation in cancellations.DefaultIfEmpty()
        select new BookingProjection(
            booking.BookingId,
            booking.FlightId,
            booking.PassengerId,
            booking.Seat == null ? null : booking.Seat.Trim(),
            booking.Price,
            flight.Departure,
            cancellation != null,
            booking.Version);

    private static Booking ToDomain(BookingProjection row) => new(
        row.BookingId,
        row.FlightId,
        row.PassengerId,
        row.Seat,
        row.Price,
        row.Departure,
        row.IsCancelled,
        row.Version);

    private static Booking ToDomain(
        BookingRow row,
        DateTimeOffset departure,
        bool isCancelled) => new(
            row.BookingId,
            row.FlightId,
            row.PassengerId,
            row.Seat?.Trim(),
            row.Price,
            departure,
            isCancelled,
            row.Version);

    private static bool IsUniqueViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation
        };

    private sealed class BookingTableEstimate
    {
        public long EstimatedRows { get; init; }
    }

    private sealed record BookingProjection(
        int BookingId,
        int FlightId,
        int PassengerId,
        string? Seat,
        decimal Price,
        DateTimeOffset Departure,
        bool IsCancelled,
        uint Version);
}
