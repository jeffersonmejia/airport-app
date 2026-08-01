namespace Airport.Features.Payments.Infrastructure.PayPal;

public sealed class PayPalOptions
{
    public const string SectionName = "PayPal";
    public const string HttpClientName = "PayPalSandbox";
    public const string SandboxBaseUrl = "https://api-m.sandbox.paypal.com/";

    public string ClientId { get; init; } = string.Empty;

    public string ClientSecret { get; init; } = string.Empty;

    public string BaseUrl { get; init; } = SandboxBaseUrl;

    public string ReturnUrl { get; init; } = string.Empty;

    public string CancelUrl { get; init; } = string.Empty;

    public bool HasValidSandboxBaseUrl =>
        Uri.TryCreate(BaseUrl, UriKind.Absolute, out var uri) &&
        uri.Scheme == Uri.UriSchemeHttps &&
        string.Equals(uri.Host, "api-m.sandbox.paypal.com", StringComparison.OrdinalIgnoreCase);

    public bool HasValidWebUrls =>
        Uri.TryCreate(ReturnUrl, UriKind.Absolute, out _) &&
        Uri.TryCreate(CancelUrl, UriKind.Absolute, out _);
}
