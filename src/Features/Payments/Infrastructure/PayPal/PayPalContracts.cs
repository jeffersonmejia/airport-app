using System.Text.Json.Serialization;

namespace Airport.Features.Payments.Infrastructure.PayPal;

internal sealed record PayPalAccessTokenResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_in")] int ExpiresIn);

internal sealed record PayPalCreateOrderPayload(
    [property: JsonPropertyName("intent")] string Intent,
    [property: JsonPropertyName("purchase_units")] IReadOnlyCollection<PayPalPurchaseUnitPayload> PurchaseUnits,
    [property: JsonPropertyName("application_context")] PayPalApplicationContextPayload ApplicationContext);

internal sealed record PayPalPurchaseUnitPayload(
    [property: JsonPropertyName("reference_id")] string ReferenceId,
    [property: JsonPropertyName("custom_id")] string CustomId,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("amount")] PayPalAmountPayload Amount);

internal sealed record PayPalAmountPayload(
    [property: JsonPropertyName("currency_code")] string CurrencyCode,
    [property: JsonPropertyName("value")] string Value);

internal sealed record PayPalApplicationContextPayload(
    [property: JsonPropertyName("shipping_preference")] string ShippingPreference,
    [property: JsonPropertyName("user_action")] string UserAction,
    [property: JsonPropertyName("return_url")] string ReturnUrl,
    [property: JsonPropertyName("cancel_url")] string CancelUrl);

internal sealed record PayPalOrderResponse(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("links")] IReadOnlyCollection<PayPalLinkResponse>? Links,
    [property: JsonPropertyName("purchase_units")] IReadOnlyCollection<PayPalPurchaseUnitResponse>? PurchaseUnits);

internal sealed record PayPalLinkResponse(
    [property: JsonPropertyName("href")] string Href,
    [property: JsonPropertyName("rel")] string Rel);

internal sealed record PayPalPurchaseUnitResponse(
    [property: JsonPropertyName("payments")] PayPalPaymentCollectionResponse? Payments);

internal sealed record PayPalPaymentCollectionResponse(
    [property: JsonPropertyName("captures")] IReadOnlyCollection<PayPalCaptureResponse>? Captures);

internal sealed record PayPalCaptureResponse(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("amount")] PayPalAmountPayload? Amount);
