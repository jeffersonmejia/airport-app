using Airport.Features.Payments.Application.CreatePayPalOrder;
using Airport.Features.Payments.Application.Ports;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Airport.Features.Payments.Presentation.Api.CreatePayPalOrder;

public static class CreatePayPalOrderEndpoint
{
    public static RouteGroupBuilder MapCreatePayPalOrder(this RouteGroupBuilder group)
    {
        group.MapPost("/orders", HandleAsync)
            .WithName("CreatePayPalOrder")
            .WithSummary("Crea una orden de pago en PayPal Sandbox")
            .Produces<CreatePayPalOrderResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status502BadGateway);

        return group;
    }

    private static async Task<IResult> HandleAsync(
        CreatePayPalOrderHttpRequest request,
        HttpRequest httpRequest,
        CreatePayPalOrderValidator validator,
        CreatePayPalOrderHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new CreatePayPalOrderCommand(
            request.Amount,
            request.CurrencyCode,
            request.ReferenceId,
            request.Description,
            httpRequest.Headers["PayPal-Request-Id"].ToString());
        var errors = validator.Validate(command);

        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        try
        {
            var response = await handler.HandleAsync(command, cancellationToken);
            return Results.Created($"/api/payments/paypal/orders/{response.OrderId}", response);
        }
        catch (PayPalGatewayException exception)
        {
            return PayPalProblemResults.GatewayFailure(exception);
        }
    }

    public sealed record CreatePayPalOrderHttpRequest(
        decimal Amount,
        string CurrencyCode,
        string ReferenceId,
        string Description);
}
