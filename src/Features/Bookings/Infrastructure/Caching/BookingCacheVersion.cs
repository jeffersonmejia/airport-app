namespace Airport.Features.Bookings.Infrastructure.Caching;

public sealed class BookingCacheVersion
{
    private long version;

    public long Current => Interlocked.Read(ref version);

    public void Advance() => Interlocked.Increment(ref version);
}
