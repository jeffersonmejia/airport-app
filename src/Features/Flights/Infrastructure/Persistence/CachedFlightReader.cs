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

    public Task<FlightSearchPage> SearchAsync(
        string? number,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var normalizedNumber = number?.Trim().ToUpperInvariant() ?? "all";
        var key = $"flights:search:{normalizedNumber}:page:{page}:size:{pageSize}";

        return cache.GetOrCreateAsync(
            key,
            token => innerReader.SearchAsync(number, page, pageSize, token),
            CachePolicy.QueryLifetime,
            cancellationToken);
    }
}
