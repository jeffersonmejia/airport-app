namespace Airport.SharedKernel.Caching;

public static class CachePolicy
{
    public const int MaximumEntries = 256;

    public static readonly TimeSpan QueryLifetime = TimeSpan.FromSeconds(30);
}
