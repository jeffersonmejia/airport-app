using System.Net;
using System.Net.Http.Json;
using Airport.Features.Auth.Presentation.Web.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Airport.Features.Auth.Presentation.Web.Services;

public sealed class AuthClient(HttpClient httpClient)
{
    public string GoogleLoginUrl => new Uri(
        httpClient.BaseAddress ?? throw new InvalidOperationException("La API no tiene dirección base."),
        "api/auth/google/login").ToString();

    public async Task<LoginResultViewModel?> LoginAsync(
        LoginCredentials credentials,
        CancellationToken cancellationToken)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/auth/login",
            credentials,
            cancellationToken);

        if (response.StatusCode is HttpStatusCode.Unauthorized)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<LoginResultViewModel>(
            cancellationToken);
    }

    public async Task<AuthProviderAvailability> GetProvidersAsync(CancellationToken cancellationToken)
    {
        using var request = CreateCookieRequest(HttpMethod.Get, "api/auth/providers");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AuthProviderAvailability>(cancellationToken)
            ?? new AuthProviderAvailability(false);
    }

    public async Task<LoginResultViewModel?> GetSessionAsync(CancellationToken cancellationToken)
    {
        using var request = CreateCookieRequest(HttpMethod.Get, "api/auth/session");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode is HttpStatusCode.Unauthorized)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<LoginResultViewModel>(cancellationToken);
    }

    public async Task<bool> CompleteMfaSignInAsync(string code, CancellationToken cancellationToken)
    {
        using var request = CreateCookieRequest(HttpMethod.Post, "api/auth/mfa/sign-in");
        request.Content = JsonContent.Create(new { code });
        using var response = await httpClient.SendAsync(request, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<MfaSetupViewModel?> GetMfaSetupAsync(CancellationToken cancellationToken)
    {
        using var request = CreateCookieRequest(HttpMethod.Get, "api/auth/mfa/setup");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MfaSetupViewModel>(cancellationToken);
    }

    public async Task<EnableMfaResult?> EnableMfaAsync(string code, CancellationToken cancellationToken)
    {
        using var request = CreateCookieRequest(HttpMethod.Post, "api/auth/mfa/enable");
        request.Content = JsonContent.Create(new { code });
        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<EnableMfaResult>(cancellationToken);
    }

    public async Task DisableMfaAsync(CancellationToken cancellationToken)
    {
        using var request = CreateCookieRequest(HttpMethod.Post, "api/auth/mfa/disable");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task LogoutAsync(CancellationToken cancellationToken)
    {
        using var request = CreateCookieRequest(HttpMethod.Post, "api/auth/logout");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode is not HttpStatusCode.Unauthorized)
        {
            response.EnsureSuccessStatusCode();
        }
    }

    private static HttpRequestMessage CreateCookieRequest(HttpMethod method, string uri)
    {
        var request = new HttpRequestMessage(method, uri);
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        return request;
    }
}
