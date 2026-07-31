using Airport.Features.Auth.Infrastructure.Security;

namespace Airport.UnitTests.Auth;

public sealed class JwtOptionsTests
{
    [Fact]
    public void AccessTokenLifetimeUsesConfiguredRange()
    {
        var options = new JwtOptions
        {
            MinimumAccessTokenMinutes = 5,
            MaximumAccessTokenMinutes = 30,
            AccessTokenMinutes = 15
        };

        Assert.True(options.HasValidAccessTokenRange);
        Assert.True(options.IsAccessTokenLifetimeAllowed);
    }

    [Fact]
    public void AccessTokenLifetimeRejectsInvalidConfiguredRange()
    {
        var options = new JwtOptions
        {
            MinimumAccessTokenMinutes = 30,
            MaximumAccessTokenMinutes = 5,
            AccessTokenMinutes = 15
        };

        Assert.False(options.HasValidAccessTokenRange);
        Assert.False(options.IsAccessTokenLifetimeAllowed);
    }
}
