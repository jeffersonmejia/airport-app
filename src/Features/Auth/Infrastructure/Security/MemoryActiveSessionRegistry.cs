using Airport.Features.Auth.Application.Ports;
using Airport.SharedKernel.Caching;

namespace Airport.Features.Auth.Infrastructure.Security;

public sealed class MemoryActiveSessionRegistry(IApplicationCache cache)
    : IActiveSessionRegistry
{
    public ValueTask ActivateAsync(
        int userId,
        string sessionId,
        TimeSpan lifetime,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        cache.Set(Key(userId), sessionId, lifetime);
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> IsActiveAsync(
        int userId,
        string sessionId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var isActive = cache.TryGet<string>(Key(userId), out var activeSessionId)
            && string.Equals(activeSessionId, sessionId, StringComparison.Ordinal);
        return ValueTask.FromResult(isActive);
    }

    public async ValueTask RevokeAsync(
        int userId,
        string sessionId,
        CancellationToken cancellationToken)
    {
        if (await IsActiveAsync(userId, sessionId, cancellationToken))
        {
            cache.Remove(Key(userId));
        }
    }

    private static string Key(int userId) => $"auth:active-session:user:{userId}";
}
