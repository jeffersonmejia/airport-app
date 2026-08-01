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

    public async Task<LoginAttemptViewModel> LoginAsync(
        LoginCredentials credentials,
        CancellationToken cancellationToken)
    {
        var isIdentityAccount = credentials.Username.Contains('@', StringComparison.Ordinal);
        using var request = isIdentityAccount
            ? CreateCookieRequest(HttpMethod.Post, "api/auth/account/login")
            : new HttpRequestMessage(HttpMethod.Post, "api/auth/login");
        request.Content = isIdentityAccount
            ? JsonContent.Create(new { email = credentials.Username, credentials.Password })
            : JsonContent.Create(new { username = credentials.Username, credentials.Password });
        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode is HttpStatusCode.Accepted)
        {
            return new LoginAttemptViewModel(null, true);
        }

        if (response.StatusCode is HttpStatusCode.Unauthorized)
        {
            return new LoginAttemptViewModel(null, false);
        }

        response.EnsureSuccessStatusCode();
        var session = await response.Content.ReadFromJsonAsync<LoginResultViewModel>(cancellationToken);
        return new LoginAttemptViewModel(session, false);
    }

    public async Task<bool> RegisterAsync(RegisterInput input, CancellationToken cancellationToken)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/auth/account/register",
            input,
            cancellationToken);
        return response.IsSuccessStatusCode;
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
        // El identificador evita que el navegador entregue un QR anterior desde
        // memoria después de regenerar la clave del autenticador.
        using var request = CreateCookieRequest(
            HttpMethod.Get,
            $"api/auth/mfa/setup?request={Guid.NewGuid():N}");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MfaSetupViewModel>(cancellationToken);
    }

    public async Task<EnableMfaAttempt> EnableMfaAsync(
        string code,
        CancellationToken cancellationToken)
    {
        using var request = CreateCookieRequest(HttpMethod.Post, "api/auth/mfa/enable");
        request.Content = JsonContent.Create(new { code });
        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<EnableMfaResult>(cancellationToken);
            return new EnableMfaAttempt(result, null);
        }

        var errorMessage = response.StatusCode switch
        {
            HttpStatusCode.Unauthorized =>
                "Tu sesión venció. Inicia sesión nuevamente antes de activar MFA.",
            HttpStatusCode.Forbidden =>
                "Tu sesión no tiene permiso para configurar MFA.",
            HttpStatusCode.BadRequest =>
                "El backend recibió el código, pero no coincide con la clave del QR. Elimina la cuenta Airport del autenticador y escanea nuevamente el QR mostrado.",
            _ =>
                $"El servidor no pudo validar MFA (HTTP {(int)response.StatusCode})."
        };

        return new EnableMfaAttempt(null, errorMessage);
    }

    public async Task DisableMfaAsync(CancellationToken cancellationToken)
    {
        using var request = CreateCookieRequest(HttpMethod.Post, "api/auth/mfa/disable");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task ResetMfaSetupAsync(CancellationToken cancellationToken)
    {
        using var request = CreateCookieRequest(HttpMethod.Post, "api/auth/mfa/reset");
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
