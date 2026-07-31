using Airport.Features.Bookings.Application;
using Airport.Features.Bookings.Application.CreateBooking;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Airport.Features.Bookings.Presentation.Api.CreateBooking;

public static class CreateBookingEndpoint
{
    public static RouteGroupBuilder MapCreateBooking(this RouteGroupBuilder group)
    {
        group.MapPost("/", HandleAsync)
            .RequireAuthorization("BookingsWrite")
            .WithName("CreateBooking")
            .Produces<BookingResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);
        return group;
    }

    private static async Task<IResult> HandleAsync(
        CreateBookingRequest request,
        CreateBookingHandler handler,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var errors = BookingValidation.ValidateMutation(
            request.FlightId, request.PassengerId, request.Seat, request.Price);
        if (errors.Count > 0) return Results.ValidationProblem(errors);

        var result = await handler.HandleAsync(
            new CreateBookingCommand(
                request.FlightId, request.PassengerId, request.Seat, request.Price),
            cancellationToken);
        return BookingMutationHttpResults.ToResult(result, timeProvider, created: true);
    }

    public sealed record CreateBookingRequest(
        int FlightId,
        int PassengerId,
        string? Seat,
        decimal Price);
}
