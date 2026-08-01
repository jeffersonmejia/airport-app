using System.Security.Claims;
using Airport.Features.Auth.Application.Login;
using Airport.Features.Auth.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;

namespace Airport.Features.Auth.Presentation.Api.Session;

public static class AuthSessionEndpoints
{
    public static RouteGroupBuilder MapSession(this RouteGroupBuilder group)
    {
        group.MapGet("/session", GetSession)
            .RequireAuthorization()
            .Produces<LoginResponse>();

        group.MapPost("/logout", LogoutAsync)
            .RequireAuthorization();

        return group;
    }

    private static IResult GetSession(ClaimsPrincipal principal)
    {
        var username = principal.Identity?.Name
            ?? principal.FindFirstValue(ClaimTypes.Email)
            ?? "usuario";
        var roles = principal.FindAll(ClaimTypes.Role)
            .Select(claim => claim.Value)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        return Results.Ok(new LoginResponse(
            string.Empty,
            "Cookie",
            DateTimeOffset.UtcNow.AddHours(8),
            username,
            roles));
    }

    private static async Task<IResult> LogoutAsync(SignInManager<ApplicationUser> signInManager)
    {
        await signInManager.SignOutAsync();
        return Results.NoContent();
    }
}
