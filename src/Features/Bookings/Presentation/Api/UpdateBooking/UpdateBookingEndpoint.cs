using Airport.Features.Bookings.Application.Ports;
using Airport.Features.Bookings.Application.UpdateBooking;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Airport.Features.Bookings.Presentation.Api.UpdateBooking;

public static class UpdateBookingEndpoint
{
    public static RouteGroupBuilder MapUpdateBooking(this RouteGroupBuilder group)
    {
        group.MapPatch("/{id:int}", HandleAsync)
            .RequireAuthorization("BookingsWrite")
            .WithName("UpdateBooking")
            .ProducesProblem(StatusCodes.Status409Conflict);
        return group;
    }

    private static async Task<IResult> HandleAsync(
        int id,
        UpdateBookingRequest request,
        UpdateBookingHandler handler,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();
        if (id <= 0) errors[nameof(id)] = ["El identificador debe ser mayor que cero."];
        if (request.Seat?.Trim().Length > 4)
        {
            errors[nameof(request.Seat)] = ["El asiento no puede superar 4 caracteres."];
        }
        if (request.Price is < 0.01m or > 99_999_999.99m)
        {
            errors[nameof(request.Price)] = ["El precio no es válido."];
        }
        if (request.Version == 0) errors[nameof(request.Version)] = ["La versión es obligatoria."];
        if (errors.Count > 0) return Results.ValidationProblem(errors);

        BookingMutationResult result = await handler.HandleAsync(
            new UpdateBookingCommand(id, request.Seat, request.Price, request.Version),
            cancellationToken);
        return BookingMutationHttpResults.ToResult(result, timeProvider);
    }

    public sealed record UpdateBookingRequest(string? Seat, decimal Price, uint Version);
}
