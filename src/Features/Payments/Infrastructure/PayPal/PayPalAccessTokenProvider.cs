using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Airport.Features.Payments.Application.Ports;
using Microsoft.Extensions.Options;

namespace Airport.Features.Payments.Infrastructure.PayPal;

internal sealed class PayPalAccessTokenProvider(
    IHttpClientFactory httpClientFactory,
    IOptions<PayPalOptions> options,
    TimeProvider timeProvider)
{
    private readonly SemaphoreSlim tokenLock = new(1, 1);
    private CachedAccessToken? cachedToken;

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        if (IsUsable(cachedToken))
        {
            return cachedToken!.Value;
        }

        await tokenLock.WaitAsync(cancellationToken);
        try
        {
            if (IsUsable(cachedToken))
            {
                return cachedToken!.Value;
            }

            cachedToken = await RequestAccessTokenAsync(cancellationToken);
            return cachedToken.Value;
        }
        finally
        {
            tokenLock.Release();
        }
    }

    private bool IsUsable(CachedAccessToken? token) =>
        token is not null && token.ExpiresAt > timeProvider.GetUtcNow().AddMinutes(1);

    private async Task<CachedAccessToken> RequestAccessTokenAsync(
        CancellationToken cancellationToken)
    {
        var settings = options.Value;
        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{settings.ClientId}:{settings.ClientSecret}"));
        using var request = new HttpRequestMessage(HttpMethod.Post, "v1/oauth2/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials"
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await httpClientFactory
            .CreateClient(PayPalOptions.HttpClientName)
            .SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw PayPalErrors.FromResponse(
                "PayPal rechazó las credenciales configuradas.",
                response);
        }

        var token = await response.Content.ReadFromJsonAsync<PayPalAccessTokenResponse>(
            cancellationToken);
        if (token is null || string.IsNullOrWhiteSpace(token.AccessToken) || token.ExpiresIn <= 0)
        {
            throw new PayPalGatewayException("PayPal devolvió una respuesta de autenticación inválida.");
        }

        return new CachedAccessToken(
            token.AccessToken,
            timeProvider.GetUtcNow().AddSeconds(token.ExpiresIn));
    }

    private sealed record CachedAccessToken(string Value, DateTimeOffset ExpiresAt);
}
