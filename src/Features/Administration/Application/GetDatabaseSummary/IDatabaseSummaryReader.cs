namespace Airport.Features.Administration.Application.GetDatabaseSummary;

public interface IDatabaseSummaryReader
{
    Task<DatabaseSummaryResponse> ReadAsync(CancellationToken cancellationToken);
}
