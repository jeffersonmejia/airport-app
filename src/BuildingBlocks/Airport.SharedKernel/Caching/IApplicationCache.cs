namespace Airport.SharedKernel.Caching;

public interface IApplicationCache
{
    bool TryGet<T>(string key, out T? value);

    void Set<T>(string key, T value, TimeSpan lifetime);

    void Remove(string key);

    Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan lifetime,
        CancellationToken cancellationToken);
}
