using Airport.Features.Bookings.Application.CancelBooking;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Airport.Features.Bookings.Presentation.Api.CancelBooking;

public static class CancelBookingEndpoint
{
    public static RouteGroupBuilder MapCancelBooking(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:int}/cancel", HandleAsync)
            .RequireAuthorization("BookingsCancel")
            .WithName("CancelBooking")
            .ProducesProblem(StatusCodes.Status409Conflict);
        return group;
    }

    private static async Task<IResult> HandleAsync(
        int id,
        CancelBookingRequest request,
        HttpContext httpContext,
        CancelBookingHandler handler,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var subject = httpContext.User.FindFirst("sub")?.Value;
        var errors = new Dictionary<string, string[]>();
        if (id <= 0) errors[nameof(id)] = ["El identificador debe ser mayor que cero."];
        if (!int.TryParse(subject, out var employeeId))
        {
            return Results.Unauthorized();
        }
        if (string.IsNullOrWhiteSpace(request.Reason) || request.Reason.Trim().Length is < 3 or > 250)
        {
            errors[nameof(request.Reason)] = ["El motivo debe contener entre 3 y 250 caracteres."];
        }
        if (errors.Count > 0) return Results.ValidationProblem(errors);

        var result = await handler.HandleAsync(
            new CancelBookingCommand(id, employeeId, request.Reason),
            cancellationToken);
        return BookingMutationHttpResults.ToResult(result, timeProvider);
    }

    public sealed record CancelBookingRequest(string Reason);
}
