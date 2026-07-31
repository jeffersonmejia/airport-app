using Airport.Core.Flights.Ports;
using Airport.Infrastructure.Features.Flights;
using Airport.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Airport.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAirportInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContextPool<AirportDbContext>(options =>
            options.UseNpgsql(connectionString));
        services.AddScoped<IFlightReader, PostgresFlightReader>();

        return services;
    }
}
