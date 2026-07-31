using Airport.Features.Flights.Application.Ports;
using Airport.Features.Flights.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Airport.Features.Flights.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFlightsInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContextPool<FlightsDbContext>(options =>
            options.UseNpgsql(connectionString));
        services.AddScoped<IFlightReader, PostgresFlightReader>();

        return services;
    }
}
