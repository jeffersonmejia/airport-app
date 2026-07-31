using System.Collections.Concurrent;
using Airport.SharedKernel.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace Airport.Caching;

public sealed class MemoryApplicationCache : IApplicationCache, IDisposable
{
    private readonly MemoryCache cache = new(new MemoryCacheOptions
    {
        SizeLimit = CachePolicy.MaximumEntries
    });
    private readonly ConcurrentDictionary<string, SemaphoreSlim> locks = new();

    public bool TryGet<T>(string key, out T? value)
    {
        if (cache.TryGetValue(key, out CacheValue<T>? cached) && cached is not null)
        {
            value = cached.Value;
            return true;
        }

        value = default;
        return false;
    }

    public void Set<T>(string key, T value, TimeSpan lifetime) =>
        cache.Set(key, new CacheValue<T>(value), new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = lifetime,
            Size = 1
        });

    public void Remove(string key) => cache.Remove(key);

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan lifetime,
        CancellationToken cancellationToken)
    {
        if (TryGet<T>(key, out var cached))
        {
            return cached!;
        }

        var gate = locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(cancellationToken);

        try
        {
            if (TryGet<T>(key, out cached))
            {
                return cached!;
            }

            var created = await factory(cancellationToken);
            Set(key, created, lifetime);
            return created;
        }
        finally
        {
            gate.Release();
            locks.TryRemove(key, out _);
        }
    }

    public void Dispose()
    {
        cache.Dispose();
        foreach (var gate in locks.Values)
        {
            gate.Dispose();
        }
    }

    private sealed record CacheValue<T>(T Value);
}
