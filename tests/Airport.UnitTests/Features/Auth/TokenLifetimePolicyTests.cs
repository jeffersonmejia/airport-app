using Airport.Features.Auth.Application.Security;

namespace Airport.UnitTests.Auth;

public sealed class TokenLifetimePolicyTests
{
    [Fact]
    public void RecommendedAccessTokenLifetime_IsFifteenMinutes()
    {
        Assert.Equal(15, TokenLifetimePolicy.RecommendedMinutes);
    }
}
