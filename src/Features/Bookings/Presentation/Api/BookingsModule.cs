using Airport.Features.Bookings.Application.CancelBooking;
using Airport.Features.Bookings.Application.CreateBooking;
using Airport.Features.Bookings.Application.GetBooking;
using Airport.Features.Bookings.Application.SearchBookings;
using Airport.Features.Bookings.Application.UpdateBooking;
using Airport.Features.Bookings.Infrastructure;
using Airport.Features.Bookings.Presentation.Api.CancelBooking;
using Airport.Features.Bookings.Presentation.Api.CreateBooking;
using Airport.Features.Bookings.Presentation.Api.GetBooking;
using Airport.Features.Bookings.Presentation.Api.SearchBookings;
using Airport.Features.Bookings.Presentation.Api.UpdateBooking;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Airport.Features.Bookings.Presentation.Api;

public static class BookingsModule
{
    public static IServiceCollection AddBookingsModule(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddScoped<SearchBookingsHandler>();
        services.AddScoped<GetBookingHandler>();
        services.AddScoped<CreateBookingHandler>();
        services.AddScoped<UpdateBookingHandler>();
        services.AddScoped<CancelBookingHandler>();
        services.AddBookingsInfrastructure(connectionString);
        return services;
    }

    public static IEndpointRouteBuilder MapBookingsModule(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGroup("/api/bookings")
            .MapSearchBookings()
            .MapGetBooking()
            .MapCreateBooking()
            .MapUpdateBooking()
            .MapCancelBooking();
        return endpoints;
    }
}
