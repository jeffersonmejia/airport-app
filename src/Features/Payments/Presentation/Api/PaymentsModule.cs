using Airport.Features.Payments.Application.CapturePayPalOrder;
using Airport.Features.Payments.Application.CreatePayPalOrder;
using Airport.Features.Payments.Infrastructure;
using Airport.Features.Payments.Presentation.Api.CapturePayPalOrder;
using Airport.Features.Payments.Presentation.Api.CreatePayPalOrder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Airport.Features.Payments.Presentation.Api;

public static class PaymentsModule
{
    public static IServiceCollection AddPaymentsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<CreatePayPalOrderValidator>();
        services.AddScoped<CreatePayPalOrderHandler>();
        services.AddSingleton<CapturePayPalOrderValidator>();
        services.AddScoped<CapturePayPalOrderHandler>();
        services.AddPaymentsInfrastructure(configuration);

        return services;
    }

    public static IEndpointRouteBuilder MapPaymentsModule(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGroup("/api/payments/paypal")
            .RequireAuthorization("AdminOnly")
            .MapCreatePayPalOrder()
            .MapCapturePayPalOrder();

        return endpoints;
    }
}
