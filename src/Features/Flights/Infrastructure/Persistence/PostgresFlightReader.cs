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
                row.OriginAirportId,
                (row.OriginAirport.Iata ?? row.OriginAirport.Icao).Trim(),
                row.OriginAirport.Name,
                row.DestinationAirportId,
                (row.DestinationAirport.Iata ?? row.DestinationAirport.Icao).Trim(),
                row.DestinationAirport.Name,
                row.Departure,
                row.Arrival,
                row.AirlineId,
                "Aerolínea " + row.AirlineId,
                row.AirplaneId))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<Airport.Features.Flights.Domain.Airport>> ListAirportsAsync(
        int? originAirportId,
        CancellationToken cancellationToken)
    {
        var availableAirportIds = originAirportId is null
            ? dbContext.Flights.Select(flight => flight.OriginAirportId).Distinct()
            : dbContext.Flights
                .Where(flight => flight.OriginAirportId == originAirportId)
                .Select(flight => flight.DestinationAirportId)
                .Distinct();

        return await dbContext.Airports
            .AsNoTracking()
            .Where(row => row.Iata != null && availableAirportIds.Contains(row.AirportId))
            .OrderBy(row => row.Name)
            .Select(row => new Airport.Features.Flights.Domain.Airport(
                row.AirportId,
                row.Iata!.Trim(),
                row.Icao.Trim(),
                row.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AirlineFilterOption>> ListAirlinesAsync(
        CancellationToken cancellationToken)
    {
        var availableAirlineIds = dbContext.Flights
            .Select(flight => flight.AirlineId)
            .Distinct();

        return await dbContext.Airlines
            .AsNoTracking()
            .Where(airline => availableAirlineIds.Contains(airline.AirlineId))
            .OrderBy(airline => airline.Name)
            .Select(airline => new AirlineFilterOption(
                airline.AirlineId,
                airline.Iata.Trim(),
                airline.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AirplaneFilterOption>> ListAirplanesAsync(
        short airlineId,
        CancellationToken cancellationToken)
    {
        var availableAirplaneIds = dbContext.Flights
            .Where(flight => flight.AirlineId == airlineId)
            .Select(flight => flight.AirplaneId)
            .Distinct();

        return await dbContext.Airplanes
            .AsNoTracking()
            .Where(airplane =>
                airplane.AirlineId == airlineId &&
                availableAirplaneIds.Contains(airplane.AirplaneId))
            .OrderBy(airplane => airplane.Type.Identifier)
            .ThenBy(airplane => airplane.AirplaneId)
            .Select(airplane => new AirplaneFilterOption(
                airplane.AirplaneId,
                airplane.Type.Identifier,
                airplane.Capacity))
            .ToListAsync(cancellationToken);
    }

    public async Task<FlightSearchPage> SearchAsync(
        FlightSearchCriteria criteria,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Flights.AsNoTracking();

        if (criteria.OriginAirportId is not null)
        {
            query = query.Where(row => row.OriginAirportId == criteria.OriginAirportId);
        }

        if (criteria.DestinationAirportId is not null)
        {
            query = query.Where(row => row.DestinationAirportId == criteria.DestinationAirportId);
        }

        if (criteria.DepartureDate is not null)
        {
            var start = new DateTimeOffset(criteria.DepartureDate.Value.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
            var end = start.AddDays(1);
            query = query.Where(row => row.Departure >= start && row.Departure < end);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Number))
        {
            query = query.Where(row => EF.Functions.ILike(row.FlightNumber, $"%{criteria.Number}%"));
        }

        if (criteria.AirlineId is not null)
        {
            query = query.Where(row => row.AirlineId == criteria.AirlineId);
        }

        if (criteria.AirplaneId is not null)
        {
            query = query.Where(row => row.AirplaneId == criteria.AirplaneId);
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var ordered = (criteria.SortBy, criteria.Descending) switch
        {
            ("arrival", true) => query.OrderByDescending(row => row.Arrival),
            ("arrival", false) => query.OrderBy(row => row.Arrival),
            ("number", true) => query.OrderByDescending(row => row.FlightNumber),
            ("number", false) => query.OrderBy(row => row.FlightNumber),
            (_, true) => query.OrderByDescending(row => row.Departure),
            _ => query.OrderBy(row => row.Departure)
        };
        var items = await ordered
            .ThenBy(row => row.FlightId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(row => new Flight(
                row.FlightId,
                row.FlightNumber,
                row.OriginAirportId,
                (row.OriginAirport.Iata ?? row.OriginAirport.Icao).Trim(),
                row.OriginAirport.Name,
                row.DestinationAirportId,
                (row.DestinationAirport.Iata ?? row.DestinationAirport.Icao).Trim(),
                row.DestinationAirport.Name,
                row.Departure,
                row.Arrival,
                row.AirlineId,
                "Aerolínea " + row.AirlineId,
                row.AirplaneId))
            .ToListAsync(cancellationToken);

        return new FlightSearchPage(items, totalItems);
    }
}
