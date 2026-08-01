using System.Net.Http.Json;
using Airport.Features.Payments.Presentation.Web.Models;

namespace Airport.Features.Payments.Presentation.Web.Services;

public sealed class PayPalPaymentsClient(HttpClient httpClient)
{
    public async Task<PayPalOrderViewModel> CreateOrderAsync(
        CreatePayPalOrderInput input,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/payments/paypal/orders")
        {
            Content = JsonContent.Create(input)
        };
        request.Headers.TryAddWithoutValidation("PayPal-Request-Id", idempotencyKey);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PayPalOrderViewModel>(cancellationToken)
            ?? throw new InvalidOperationException("La API devolvió una orden PayPal vacía.");
    }

    public async Task<PayPalCaptureViewModel> CaptureOrderAsync(
        string orderId,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"api/payments/paypal/orders/{Uri.EscapeDataString(orderId)}/capture");
        request.Headers.TryAddWithoutValidation("PayPal-Request-Id", idempotencyKey);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PayPalCaptureViewModel>(cancellationToken)
            ?? throw new InvalidOperationException("La API devolvió una captura PayPal vacía.");
    }
}
