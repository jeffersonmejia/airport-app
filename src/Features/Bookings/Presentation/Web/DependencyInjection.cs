using Airport.Features.Bookings.Presentation.Web.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Airport.Features.Bookings.Presentation.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddBookingsPresentation(this IServiceCollection services)
    {
        services.AddScoped<BookingsClient>();
        return services;
    }
}
