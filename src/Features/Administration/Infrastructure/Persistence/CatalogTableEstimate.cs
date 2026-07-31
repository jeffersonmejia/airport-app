namespace Airport.Features.Administration.Infrastructure.Persistence;

public sealed class CatalogTableEstimate
{
    public string TableName { get; init; } = string.Empty;
    public long EstimatedRows { get; init; }
}
