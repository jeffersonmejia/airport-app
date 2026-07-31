using Airport.Features.Flights.Application.Ports;
using Airport.Features.Flights.Domain;
using Microsoft.EntityFrameworkCore;

namespace Airport.Features.Flights.Infrastructure.Persistence;

public sealed class PostgresFlightReader(FlightsDbContext dbContext) : IFlightReader
{
    public Task<Flight?> FindByIdAsync(int id, CancellationToken cancellationToken) =>
        dbContext.Flights
            .AsNoTracking()
            .Where(row => row.FlightId == id)
            .Select(row => new Flight(
                row.FlightId,
                row.FlightNumber,
                row.Departure,
                row.Arrival,
                row.AirlineId,
                row.AirplaneId))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<FlightSearchPage> SearchAsync(
        string? number,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Flights.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(number))
        {
            query = query.Where(row => EF.Functions.ILike(row.FlightNumber, $"%{number}%"));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(row => row.Departure)
            .ThenBy(row => row.FlightId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(row => new Flight(
                row.FlightId,
                row.FlightNumber,
                row.Departure,
                row.Arrival,
                row.AirlineId,
                row.AirplaneId))
            .ToListAsync(cancellationToken);

        return new FlightSearchPage(items, totalItems);
    }
}
