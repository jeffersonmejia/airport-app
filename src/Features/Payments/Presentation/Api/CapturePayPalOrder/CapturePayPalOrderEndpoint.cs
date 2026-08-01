using Airport.Features.Payments.Application.CapturePayPalOrder;
using Airport.Features.Payments.Application.Ports;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Airport.Features.Payments.Presentation.Api.CapturePayPalOrder;

public static class CapturePayPalOrderEndpoint
{
    public static RouteGroupBuilder MapCapturePayPalOrder(this RouteGroupBuilder group)
    {
        group.MapPost("/orders/{orderId}/capture", HandleAsync)
            .WithName("CapturePayPalOrder")
            .WithSummary("Captura una orden aprobada en PayPal Sandbox")
            .Produces<CapturePayPalOrderResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status502BadGateway);

        return group;
    }

    private static async Task<IResult> HandleAsync(
        string orderId,
        HttpRequest httpRequest,
        ClaimsPrincipal principal,
        CapturePayPalOrderValidator validator,
        CapturePayPalOrderHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new CapturePayPalOrderCommand(
            orderId,
            principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
            httpRequest.Headers["PayPal-Request-Id"].ToString());
        var errors = validator.Validate(command);

        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        try
        {
            var response = await handler.HandleAsync(command, cancellationToken);
            return Results.Ok(response);
        }
        catch (PayPalGatewayException exception)
        {
            return PayPalProblemResults.GatewayFailure(exception);
        }
        catch (PaymentOrderException exception)
        {
            return Results.Problem(
                title: "La captura no puede completarse",
                detail: exception.Message,
                statusCode: StatusCodes.Status422UnprocessableEntity);
        }
    }
}
