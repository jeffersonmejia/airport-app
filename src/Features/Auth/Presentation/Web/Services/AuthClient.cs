using System.Net;
using System.Net.Http.Json;
using Airport.Features.Auth.Presentation.Web.Models;

namespace Airport.Features.Auth.Presentation.Web.Services;

public sealed class AuthClient(HttpClient httpClient)
{
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
}
