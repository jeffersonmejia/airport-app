using Airport.Features.Bookings.Application.Ports;
using Airport.Features.Bookings.Infrastructure.Persistence;
using Airport.Features.Payments.Application.Ports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Airport.Features.Bookings.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBookingsInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContextPool<BookingsDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IBookingRepository, PostgresBookingRepository>();
        services.AddScoped<IPaymentOrderStore, PostgresPaymentOrderStore>();
        return services;
    }
}
