using System.Text;
using Airport.Features.Auth.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using QRCoder;

namespace Airport.Features.Auth.Presentation.Api.Mfa;

public static class MfaEndpoints
{
    private const string Issuer = "Airport";

    public static RouteGroupBuilder MapMfa(this RouteGroupBuilder group)
    {
        group.MapGet("/mfa/setup", GetSetupAsync)
            .RequireAuthorization(policy => policy
                .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme)
                .RequireAuthenticatedUser());

        group.MapPost("/mfa/enable", EnableAsync)
            .RequireAuthorization(policy => policy
                .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme)
                .RequireAuthenticatedUser());

        group.MapPost("/mfa/disable", DisableAsync)
            .RequireAuthorization(policy => policy
                .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme)
                .RequireAuthenticatedUser());

        group.MapPost("/mfa/sign-in", SignInAsync)
            .AllowAnonymous();

        return group;
    }

    private static async Task<IResult> GetSetupAsync(
        HttpContext context,
        UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.GetUserAsync(context.User);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        var key = await userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrWhiteSpace(key))
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            key = await userManager.GetAuthenticatorKeyAsync(user);
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            return Results.Problem("No fue posible generar la clave MFA.");
        }

        var accountName = user.Email ?? user.UserName ?? user.Id;
        var authenticatorUri = BuildAuthenticatorUri(accountName, key);
        var qrCodeDataUri = BuildQrCodeDataUri(authenticatorUri);

        return Results.Ok(new MfaSetupResponse(
            FormatKey(key),
            authenticatorUri,
            qrCodeDataUri,
            await userManager.GetTwoFactorEnabledAsync(user)));
    }

    private static async Task<IResult> EnableAsync(
        MfaCodeRequest request,
        HttpContext context,
        UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.GetUserAsync(context.User);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        var code = NormalizeCode(request.Code);
        var valid = await userManager.VerifyTwoFactorTokenAsync(
            user,
            TokenOptions.DefaultAuthenticatorProvider,
            code);

        if (!valid)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(request.Code)] = ["El código del autenticador no es válido."]
            });
        }

        await userManager.SetTwoFactorEnabledAsync(user, true);
        var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 8);
        return Results.Ok(new { recoveryCodes = recoveryCodes?.ToArray() ?? [] });
    }

    private static async Task<IResult> DisableAsync(
        HttpContext context,
        UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.GetUserAsync(context.User);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        await userManager.SetTwoFactorEnabledAsync(user, false);
        await userManager.ResetAuthenticatorKeyAsync(user);
        return Results.NoContent();
    }

    private static async Task<IResult> SignInAsync(
        MfaCodeRequest request,
        SignInManager<ApplicationUser> signInManager)
    {
        var result = await signInManager.TwoFactorAuthenticatorSignInAsync(
            NormalizeCode(request.Code),
            isPersistent: false,
            rememberClient: false);

        return result.Succeeded
            ? Results.NoContent()
            : Results.Problem(
                title: "Código MFA inválido",
                statusCode: StatusCodes.Status401Unauthorized);
    }

    private static string BuildAuthenticatorUri(string accountName, string key) =>
        $"otpauth://totp/{Uri.EscapeDataString(Issuer)}:{Uri.EscapeDataString(accountName)}" +
        $"?secret={key}&issuer={Uri.EscapeDataString(Issuer)}&digits=6";

    private static string BuildQrCodeDataUri(string authenticatorUri)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(authenticatorUri, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new SvgQRCode(data);
        var svg = qrCode.GetGraphic(6);
        return $"data:image/svg+xml;base64,{Convert.ToBase64String(Encoding.UTF8.GetBytes(svg))}";
    }

    private static string FormatKey(string key) => string.Join(
        ' ',
        Enumerable.Range(0, (key.Length + 3) / 4)
            .Select(index => key.Substring(index * 4, Math.Min(4, key.Length - index * 4)))
            .Select(part => part.ToLowerInvariant()));

    private static string NormalizeCode(string? code) =>
        (code ?? string.Empty).Replace(" ", string.Empty).Replace("-", string.Empty);

    private sealed record MfaCodeRequest(string Code);

    private sealed record MfaSetupResponse(
        string SharedKey,
        string AuthenticatorUri,
        string QrCodeDataUri,
        bool IsEnabled);
}
