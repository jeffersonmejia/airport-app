using Airport.Features.Bookings.Application.Ports;
using Airport.Features.Bookings.Domain;
using Airport.Features.Bookings.Infrastructure.Persistence;
using Airport.SharedKernel.Caching;

namespace Airport.Features.Bookings.Infrastructure.Caching;

public sealed class CachedBookingRepository(
    PostgresBookingRepository inner,
    IApplicationCache cache,
    BookingCacheVersion cacheVersion) : IBookingRepository
{
    public Task<BookingPage> SearchAsync(
        int? bookingId,
        int? flightId,
        int? passengerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync(
            $"bookings:{cacheVersion.Current}:search:{bookingId}:{flightId}:{passengerId}:{page}:{pageSize}",
            token => inner.SearchAsync(bookingId, flightId, passengerId, page, pageSize, token),
            CachePolicy.QueryLifetime,
            cancellationToken);

    public Task<Booking?> FindByIdAsync(int bookingId, CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync(
            $"bookings:{cacheVersion.Current}:detail:{bookingId}",
            token => inner.FindByIdAsync(bookingId, token),
            CachePolicy.QueryLifetime,
            cancellationToken);

    public async Task<BookingMutationResult> CreateAsync(
        int flightId,
        int passengerId,
        string? seat,
        decimal price,
        CancellationToken cancellationToken)
    {
        var result = await inner.CreateAsync(
            flightId, passengerId, seat, price, cancellationToken);
        AdvanceOnSuccess(result);
        return result;
    }

    public async Task<BookingMutationResult> UpdateAsync(
        int bookingId,
        string? seat,
        decimal price,
        uint version,
        CancellationToken cancellationToken)
    {
        var result = await inner.UpdateAsync(
            bookingId, seat, price, version, cancellationToken);
        AdvanceOnSuccess(result);
        return result;
    }

    public async Task<BookingMutationResult> CancelAsync(
        int bookingId,
        int employeeId,
        string reason,
        CancellationToken cancellationToken)
    {
        var result = await inner.CancelAsync(
            bookingId, employeeId, reason, cancellationToken);
        AdvanceOnSuccess(result);
        return result;
    }

    private void AdvanceOnSuccess(BookingMutationResult result)
    {
        if (result.Status is BookingMutationStatus.Success)
        {
            cacheVersion.Advance();
        }
    }
}
