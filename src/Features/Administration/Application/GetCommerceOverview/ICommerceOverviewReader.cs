namespace Airport.Features.Administration.Application.GetCommerceOverview;

public interface ICommerceOverviewReader
{
    Task<CommerceOverviewResponse> ReadAsync(int page, int pageSize, CancellationToken cancellationToken);
}
