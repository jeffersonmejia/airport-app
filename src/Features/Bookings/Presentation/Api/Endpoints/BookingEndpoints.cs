using System.Security.Claims;
using Airport.Features.Bookings.Application.CreateOrder;
using Airport.Features.Bookings.Application.GetHistory;
using Airport.Features.Bookings.Application.GetOrder;
using Airport.Features.Bookings.Application.GetReceipt;
using Airport.SharedKernel.Pagination;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Airport.Features.Bookings.Presentation.Api.Endpoints;

public static class BookingEndpoints
{
    public static RouteGroupBuilder MapBookingEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/orders", CreateAsync).Produces<CreateOrderResponse>(StatusCodes.Status201Created);
        group.MapGet("/orders/{orderId:guid}", GetAsync).Produces<CreateOrderResponse>();
        group.MapGet("/history", GetHistoryAsync).Produces<BookingHistoryResponse>();
        group.MapGet("/orders/{orderId:guid}/receipt", GetReceiptAsync).Produces<ReceiptResponse>();
        return group;
    }

    private static async Task<IResult> CreateAsync(
        CreateOrderRequest request,
        ClaimsPrincipal principal,
        CreateOrderValidator validator,
        CreateOrderHandler handler,
        CancellationToken cancellationToken)
    {
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var command = new CreateOrderCommand(
            userId,
            request.FlightId,
            request.FareCode,
            request.PassengerFirstName,
            request.PassengerLastName,
            request.PassportNumber);
        var errors = validator.Validate(command);
        if (errors.Count > 0) return Results.ValidationProblem(errors);

        var response = await handler.HandleAsync(command, cancellationToken);
        return response is null
            ? Results.Problem(
                title: "Vuelo o tarifa no disponible",
                statusCode: StatusCodes.Status422UnprocessableEntity)
            : Results.Created($"/api/bookings/orders/{response.OrderId}", response);
    }

    private static async Task<IResult> GetAsync(
        Guid orderId,
        ClaimsPrincipal principal,
        GetOrderHandler handler,
        CancellationToken cancellationToken)
    {
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var response = await handler.HandleAsync(orderId, userId, cancellationToken);
        return response is null ? Results.NotFound() : Results.Ok(response);
    }

    private static async Task<IResult> GetHistoryAsync(
        int? page,
        int? pageSize,
        ClaimsPrincipal principal,
        GetHistoryHandler handler,
        CancellationToken cancellationToken)
    {
        var selectedPage = page ?? 1;
        var selectedSize = pageSize ?? PaginationPolicy.PageSize;
        if (selectedPage < 1 || selectedSize is < 1 or > PaginationPolicy.PageSize)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(page)] = ["La página o su tamaño no son válidos."]
            });
        }
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        return Results.Ok(await handler.HandleAsync(
            userId,
            selectedPage,
            selectedSize,
            cancellationToken));
    }

    private static async Task<IResult> GetReceiptAsync(
        Guid orderId,
        ClaimsPrincipal principal,
        GetReceiptHandler handler,
        CancellationToken cancellationToken)
    {
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var response = await handler.HandleAsync(orderId, userId, cancellationToken);
        return response is null ? Results.NotFound() : Results.Ok(response);
    }

    private sealed record CreateOrderRequest(
        int FlightId,
        string FareCode,
        string PassengerFirstName,
        string PassengerLastName,
        string PassportNumber);
}
