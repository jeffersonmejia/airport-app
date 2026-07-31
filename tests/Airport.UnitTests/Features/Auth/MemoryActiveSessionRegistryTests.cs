using Airport.Caching;
using Airport.Features.Auth.Infrastructure.Security;

namespace Airport.UnitTests.Auth;

public sealed class MemoryActiveSessionRegistryTests
{
    [Fact]
    public async Task ActivateAsync_ReplacesThePreviousSessionForTheUser()
    {
        using var cache = new MemoryApplicationCache();
        var registry = new MemoryActiveSessionRegistry(cache);

        await registry.ActivateAsync(7, "first", TimeSpan.FromMinutes(15), CancellationToken.None);
        await registry.ActivateAsync(7, "second", TimeSpan.FromMinutes(15), CancellationToken.None);

        Assert.False(await registry.IsActiveAsync(7, "first", CancellationToken.None));
        Assert.True(await registry.IsActiveAsync(7, "second", CancellationToken.None));
    }
}
