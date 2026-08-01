using System.Net.Http.Headers;
using Airport.Features.Auth.Presentation.Web.Models;

namespace Airport.Features.Auth.Presentation.Web.Services;

public sealed class AuthSession(HttpClient httpClient)
{
    public event Action? Changed;

    public LoginResultViewModel? Current { get; private set; }

    public bool IsAuthenticated => Current is not null && Current.ExpiresAt > DateTimeOffset.UtcNow;

    public bool IsAdmin => IsAuthenticated && Current!.Roles.Contains("Admin", StringComparer.Ordinal);

    public bool IsCookieSession => IsAuthenticated &&
        string.Equals(Current!.TokenType, "Cookie", StringComparison.Ordinal);

    public bool IsInRole(string role) =>
        IsAuthenticated && Current!.Roles.Contains(role, StringComparer.Ordinal);

    public void SignIn(LoginResultViewModel session)
    {
        Current = session;
        httpClient.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(session.AccessToken)
            ? null
            : new AuthenticationHeaderValue(session.TokenType, session.AccessToken);
        Changed?.Invoke();
    }

    public void SignOut()
    {
        Current = null;
        httpClient.DefaultRequestHeaders.Authorization = null;
        Changed?.Invoke();
    }
}
