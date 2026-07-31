using Airport.Core.Flights.Domain;
using Airport.Core.Flights.Ports;
using Airport.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Airport.Infrastructure.Features.Flights;

public sealed class PostgresFlightReader(AirportDbContext dbContext) : IFlightReader
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
}
