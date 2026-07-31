using Airport.Features.Bookings.Application;
using Airport.Features.Bookings.Application.GetBooking;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Airport.Features.Bookings.Presentation.Api.GetBooking;

public static class GetBookingEndpoint
{
    public static RouteGroupBuilder MapGetBooking(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}", HandleAsync)
            .RequireAuthorization("BookingsRead")
            .WithName("GetBooking")
            .Produces<BookingResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> HandleAsync(
        int id,
        GetBookingHandler handler,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(id)] = ["El identificador debe ser mayor que cero."]
            });
        }

        var booking = await handler.HandleAsync(id, cancellationToken);
        return booking is null
            ? Results.Problem(
                title: "Reserva no encontrada",
                detail: "La reserva solicitada no existe.",
                statusCode: StatusCodes.Status404NotFound)
            : Results.Ok(booking);
    }
}
