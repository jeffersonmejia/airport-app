using Airport.Features.Payments.Application.Ports;

namespace Airport.Features.Payments.Infrastructure.PayPal;

internal static class PayPalErrors
{
    public static PayPalGatewayException FromResponse(
        string message,
        HttpResponseMessage response)
    {
        var debugId = response.Headers.TryGetValues("PayPal-Debug-Id", out var values)
            ? values.FirstOrDefault()
            : null;

        return new PayPalGatewayException(message, (int)response.StatusCode, debugId);
    }
}
