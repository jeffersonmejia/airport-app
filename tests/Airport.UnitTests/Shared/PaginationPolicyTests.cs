using Airport.SharedKernel.Pagination;

namespace Airport.UnitTests.Shared;

public sealed class PaginationPolicyTests
{
    [Fact]
    public void PageSize_IsLimitedToFiveItems()
    {
        Assert.Equal(5, PaginationPolicy.PageSize);
    }
}
