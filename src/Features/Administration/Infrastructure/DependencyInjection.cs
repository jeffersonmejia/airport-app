using Airport.Features.Administration.Application.GetDatabaseSummary;
using Airport.Features.Administration.Application.GetCommerceOverview;
using Airport.Features.Administration.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Airport.Features.Administration.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAdministrationInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddDbContextPool<AdministrationDbContext>(options =>
            options.UseNpgsql(connectionString));
        services.AddScoped<IDatabaseSummaryReader, PostgresDatabaseSummaryReader>();
        services.AddScoped<ICommerceOverviewReader, PostgresCommerceOverviewReader>();
        return services;
    }
}
