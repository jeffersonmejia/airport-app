using System.Security.Claims;
using Airport.Features.Auth.Application.Roles;
using Airport.Features.Auth.Infrastructure.Persistence;
using Airport.Features.Auth.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;

namespace Airport.Features.Auth.Presentation.Api.Google;

public static class GoogleAuthEndpoints
{
    public static RouteGroupBuilder MapGoogleAuth(this RouteGroupBuilder group)
    {
        group.MapGet("/providers", (GoogleAuthOptions google) =>
                Results.Ok(new { google = google.IsConfigured }))
            .AllowAnonymous();

        group.MapGet("/google/login", StartAsync)
            .AllowAnonymous();

        group.MapGet("/google/callback", CompleteAsync)
            .AllowAnonymous();

        return group;
    }

    private static IResult StartAsync(GoogleAuthOptions google)
    {
        if (!google.IsConfigured)
        {
            return Results.Problem(
                title: "Google no está configurado",
                detail: "Configura Authentication:Google en User Secrets.",
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        var properties = new AuthenticationProperties
        {
            RedirectUri = "/api/auth/google/callback"
        };

        return Results.Challenge(properties, [GoogleDefaults.AuthenticationScheme]);
    }

    private static async Task<IResult> CompleteAsync(
        GoogleAuthOptions google,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        if (!google.IsConfigured)
        {
            return Results.Problem(statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            return RedirectWithStatus(google.WebCallbackUrl, "error");
        }

        var result = await signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: false);

        if (result.Succeeded)
        {
            return RedirectWithStatus(google.WebCallbackUrl, "success");
        }

        if (result.RequiresTwoFactor)
        {
            return RedirectWithStatus(google.WebCallbackUrl, "mfa");
        }

        if (result.IsLockedOut)
        {
            return RedirectWithStatus(google.WebCallbackUrl, "locked");
        }

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email))
        {
            return RedirectWithStatus(google.WebCallbackUrl, "email_required");
        }

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            return RedirectWithStatus(google.WebCallbackUrl, "account_exists");
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var creation = await userManager.CreateAsync(user);
        if (!creation.Succeeded)
        {
            return RedirectWithStatus(google.WebCallbackUrl, "error");
        }

        if (!await roleManager.RoleExistsAsync(ApplicationRoles.Client))
        {
            var roleCreation = await roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Client));
            if (!roleCreation.Succeeded)
            {
                await userManager.DeleteAsync(user);
                return RedirectWithStatus(google.WebCallbackUrl, "error");
            }
        }

        var role = await userManager.AddToRoleAsync(user, ApplicationRoles.Client);
        var login = await userManager.AddLoginAsync(user, info);
        if (!role.Succeeded || !login.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return RedirectWithStatus(google.WebCallbackUrl, "error");
        }

        await signInManager.SignInAsync(user, isPersistent: false);
        return RedirectWithStatus(google.WebCallbackUrl, "success");
    }

    private static IResult RedirectWithStatus(string callbackUrl, string status)
    {
        var separator = callbackUrl.Contains('?', StringComparison.Ordinal) ? '&' : '?';
        return Results.Redirect($"{callbackUrl}{separator}status={Uri.EscapeDataString(status)}");
    }
}
