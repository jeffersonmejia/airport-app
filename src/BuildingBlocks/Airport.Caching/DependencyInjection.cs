using Airport.SharedKernel.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Airport.Caching;

public static class DependencyInjection
{
    public static IServiceCollection AddAirportCaching(this IServiceCollection services)
    {
        services.AddSingleton<IApplicationCache, MemoryApplicationCache>();
        return services;
    }
}
