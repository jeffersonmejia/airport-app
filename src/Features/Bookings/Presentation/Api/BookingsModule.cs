using Airport.Features.Bookings.Application.CreateOrder;
using Airport.Features.Bookings.Application.GetHistory;
using Airport.Features.Bookings.Application.GetOrder;
using Airport.Features.Bookings.Application.GetReceipt;
using Airport.Features.Bookings.Infrastructure;
using Airport.Features.Bookings.Presentation.Api.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Airport.Features.Bookings.Presentation.Api;

public static class BookingsModule
{
    public static IServiceCollection AddBookingsModule(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<CreateOrderValidator>();
        services.AddScoped<CreateOrderHandler>();
        services.AddScoped<GetOrderHandler>();
        services.AddScoped<GetHistoryHandler>();
        services.AddScoped<GetReceiptHandler>();
        services.AddBookingsInfrastructure(connectionString);
        return services;
    }

    public static IEndpointRouteBuilder MapBookingsModule(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGroup("/api/bookings")
            .RequireAuthorization("ClientOnly")
            .MapBookingEndpoints();
        return endpoints;
    }
}
