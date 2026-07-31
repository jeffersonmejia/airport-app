namespace Airport.Features.Administration.Presentation.Web.Models;

public sealed record DatabaseSummaryViewModel(
    long Total,
    bool IsApproximate,
    DateTimeOffset UpdatedAt,
    IReadOnlyCollection<DatabaseSummaryItemViewModel> Items);

public sealed record DatabaseSummaryItemViewModel(string Label, long Count);
