using Airport.Features.Bookings.Application;
using Airport.Features.Bookings.Application.SearchBookings;
using Airport.SharedKernel.Pagination;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Airport.Features.Bookings.Presentation.Api.SearchBookings;

public static class SearchBookingsEndpoint
{
    public static RouteGroupBuilder MapSearchBookings(this RouteGroupBuilder group)
    {
        group.MapGet("/", HandleAsync)
            .RequireAuthorization("BookingsRead")
            .WithName("SearchBookings")
            .Produces<SearchBookingsResponse>()
            .ProducesValidationProblem();
        return group;
    }

    private static async Task<IResult> HandleAsync(
        int? bookingId,
        int? flightId,
        int? passengerId,
        int? page,
        int? pageSize,
        SearchBookingsHandler handler,
        CancellationToken cancellationToken)
    {
        var currentPage = page ?? PaginationPolicy.DefaultPage;
        var currentPageSize = pageSize ?? PaginationPolicy.PageSize;
        var errors = new Dictionary<string, string[]>(
            BookingValidation.ValidatePage(currentPage, currentPageSize));
        AddIdError(errors, nameof(bookingId), bookingId);
        AddIdError(errors, nameof(flightId), flightId);
        AddIdError(errors, nameof(passengerId), passengerId);

        if (errors.Count > 0) return Results.ValidationProblem(errors);

        var response = await handler.HandleAsync(
            new SearchBookingsQuery(
                bookingId, flightId, passengerId, currentPage, currentPageSize),
            cancellationToken);
        return Results.Ok(response);
    }

    private static void AddIdError(
        Dictionary<string, string[]> errors,
        string name,
        int? value)
    {
        if (value is <= 0) errors[name] = ["El identificador debe ser mayor que cero."];
    }
}
