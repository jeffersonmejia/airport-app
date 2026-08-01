using Airport.Features.Flights.Application.Ports;
using Airport.Features.Flights.Domain;
using Airport.SharedKernel.Caching;

namespace Airport.Features.Flights.Infrastructure.Persistence;

public sealed class CachedFlightReader(
    PostgresFlightReader innerReader,
    IApplicationCache cache) : IFlightReader
{
    public Task<Flight?> FindByIdAsync(int id, CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync(
            $"flights:id:{id}",
            token => innerReader.FindByIdAsync(id, token),
            CachePolicy.QueryLifetime,
            cancellationToken);

    public Task<IReadOnlyList<Airport.Features.Flights.Domain.Airport>> ListAirportsAsync(
        int? originAirportId,
        CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync(
            $"flights:airports:origin:{originAirportId?.ToString() ?? "all"}",
            token => innerReader.ListAirportsAsync(originAirportId, token),
            CachePolicy.QueryLifetime,
            cancellationToken);

    public Task<IReadOnlyList<AirlineFilterOption>> ListAirlinesAsync(
        CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync(
            "flights:filter-options:airlines",
            innerReader.ListAirlinesAsync,
            CachePolicy.QueryLifetime,
            cancellationToken);

    public Task<IReadOnlyList<AirplaneFilterOption>> ListAirplanesAsync(
        short airlineId,
        CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync(
            $"flights:filter-options:airplanes:airline:{airlineId}",
            token => innerReader.ListAirplanesAsync(airlineId, token),
            CachePolicy.QueryLifetime,
            cancellationToken);

    public Task<IReadOnlyList<string>> ListFlightNumbersAsync(
        FlightNumberFilter filter,
        CancellationToken cancellationToken)
    {
        var key = $"flights:filter-options:numbers:{filter.OriginAirportId}:" +
            $"{filter.DestinationAirportId}:{filter.DepartureDate:yyyy-MM-dd}:" +
            $"{filter.AirlineId}:{filter.AirplaneId}";

        return cache.GetOrCreateAsync(
            key,
            token => innerReader.ListFlightNumbersAsync(filter, token),
            CachePolicy.QueryLifetime,
            cancellationToken);
    }

    public Task<FlightSearchPage> SearchAsync(
        FlightSearchCriteria criteria,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var normalizedNumber = criteria.Number?.Trim().ToUpperInvariant() ?? "all";
        var key = $"flights:search:{criteria.OriginAirportId}:{criteria.DestinationAirportId}:" +
            $"{criteria.DepartureDate:yyyy-MM-dd}:{normalizedNumber}:{criteria.SortBy}:" +
            $"{criteria.Descending}:airline:{criteria.AirlineId}:airplane:{criteria.AirplaneId}:" +
            $"page:{page}:size:{pageSize}";

        return cache.GetOrCreateAsync(
            key,
            token => innerReader.SearchAsync(criteria, page, pageSize, token),
            CachePolicy.QueryLifetime,
            cancellationToken);
    }
}
