using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Airport.Features.Payments.Application.Ports;
using Airport.Features.Payments.Domain;
using Microsoft.Extensions.Options;

namespace Airport.Features.Payments.Infrastructure.PayPal;

internal sealed class PayPalPaymentGateway(
    IHttpClientFactory httpClientFactory,
    PayPalAccessTokenProvider accessTokenProvider,
    IOptions<PayPalOptions> options) : IPayPalGateway
{
    private readonly PayPalOptions settings = options.Value;
    public async Task<PayPalOrder> CreateOrderAsync(
        CreatePayPalOrderRequest request,
        CancellationToken cancellationToken)
    {
        var payload = new PayPalCreateOrderPayload(
            "CAPTURE",
            [new PayPalPurchaseUnitPayload(
                request.ReferenceId,
                request.ReferenceId,
                request.Description,
                new PayPalAmountPayload(
                    request.Amount.CurrencyCode,
                    request.Amount.ToPayPalValue()))],
            new PayPalApplicationContextPayload(
                "NO_SHIPPING",
                "PAY_NOW",
                settings.ReturnUrl,
                settings.CancelUrl));
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "v2/checkout/orders")
        {
            Content = JsonContent.Create(payload)
        };
        httpRequest.Headers.TryAddWithoutValidation("PayPal-Request-Id", request.IdempotencyKey);

        var response = await SendAsync(httpRequest, cancellationToken);
        var approvalUrl = response.Links?
            .FirstOrDefault(link => string.Equals(link.Rel, "approve", StringComparison.OrdinalIgnoreCase))?
            .Href;

        return new PayPalOrder(
            response.Id,
            response.Status,
            Uri.TryCreate(approvalUrl, UriKind.Absolute, out var uri) ? uri : null);
    }

    public async Task<PayPalCapture> CaptureOrderAsync(
        string orderId,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"v2/checkout/orders/{Uri.EscapeDataString(orderId)}/capture")
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        };
        httpRequest.Headers.TryAddWithoutValidation("PayPal-Request-Id", idempotencyKey);

        var response = await SendAsync(httpRequest, cancellationToken);
        var capture = response.PurchaseUnits?
            .SelectMany(unit => unit.Payments?.Captures ?? [])
            .FirstOrDefault();
        PaymentMoney? amount = null;
        if (capture?.Amount is not null)
        {
            if (!decimal.TryParse(
                    capture.Amount.Value,
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var capturedValue))
            {
                throw new PayPalGatewayException("PayPal devolvió un monto capturado inválido.");
            }

            amount = PaymentMoney.Create(capturedValue, capture.Amount.CurrencyCode);
        }

        return new PayPalCapture(
            response.Id,
            response.Status,
            capture?.Id,
            amount);
    }

    private async Task<PayPalOrderResponse> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var accessToken = await accessTokenProvider.GetAccessTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await httpClientFactory
            .CreateClient(PayPalOptions.HttpClientName)
            .SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw PayPalErrors.FromResponse(
                "PayPal no pudo procesar la operación solicitada.",
                response);
        }

        var result = await response.Content.ReadFromJsonAsync<PayPalOrderResponse>(
            cancellationToken);

        return result is null || string.IsNullOrWhiteSpace(result.Id)
            ? throw new PayPalGatewayException("PayPal devolvió una respuesta de pago inválida.")
            : result;
    }
}
