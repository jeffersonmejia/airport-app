using Airport.Features.Bookings.Application.Ports;
using Airport.Features.Bookings.Infrastructure.Caching;
using Airport.Features.Bookings.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Airport.Features.Bookings.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBookingsInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<BookingCacheVersion>();
        services.AddDbContextPool<BookingsDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<PostgresBookingRepository>();
        services.AddScoped<IBookingRepository, CachedBookingRepository>();
        return services;
    }
}
