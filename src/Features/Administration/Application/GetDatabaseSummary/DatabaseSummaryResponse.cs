namespace Airport.Features.Administration.Application.GetDatabaseSummary;

public sealed record DatabaseSummaryResponse(
    long Total,
    bool IsApproximate,
    DateTimeOffset UpdatedAt,
    IReadOnlyCollection<DatabaseSummaryItem> Items);
