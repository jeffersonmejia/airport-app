using Airport.Features.Payments.Presentation.Web.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Airport.Features.Payments.Presentation.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentsPresentation(this IServiceCollection services)
    {
        services.AddScoped<PayPalPaymentsClient>();
        return services;
    }
}
