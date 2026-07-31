using Airport.SharedKernel.Caching;

namespace Airport.Features.Administration.Application.GetDatabaseSummary;

public sealed class GetDatabaseSummaryHandler(
    IDatabaseSummaryReader reader,
    IApplicationCache cache)
{
    private const string CacheKey = "administration:database-summary";

    public Task<DatabaseSummaryResponse> HandleAsync(CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync(
            CacheKey,
            reader.ReadAsync,
            CachePolicy.QueryLifetime,
            cancellationToken);
}
