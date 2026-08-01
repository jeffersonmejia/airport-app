using Airport.Features.Payments.Application.Ports;
using Microsoft.AspNetCore.Http;

namespace Airport.Features.Payments.Presentation.Api;

internal static class PayPalProblemResults
{
    public static IResult GatewayFailure(PayPalGatewayException exception)
    {
        var wasRejected = exception.StatusCode is 400 or 409 or 422;
        return Results.Problem(
            title: "Pago no procesado",
            detail: wasRejected
                ? "PayPal rechazó la operación solicitada."
                : "PayPal no pudo completar la operación. Inténtalo nuevamente.",
            statusCode: wasRejected
                ? StatusCodes.Status422UnprocessableEntity
                : StatusCodes.Status502BadGateway,
            extensions: exception.DebugId is null
                ? null
                : new Dictionary<string, object?> { ["paypalDebugId"] = exception.DebugId });
    }
}
