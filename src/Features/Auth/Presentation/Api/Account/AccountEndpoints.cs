using System.ComponentModel.DataAnnotations;
using System.Text;
using Airport.Features.Auth.Application.Login;
using Airport.Features.Auth.Application.Ports;
using Airport.Features.Auth.Application.Roles;
using Airport.Features.Auth.Infrastructure.Persistence;
using Airport.Features.Auth.Infrastructure.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;

namespace Airport.Features.Auth.Presentation.Api.Account;

public static class AccountEndpoints
{
    public static RouteGroupBuilder MapAccount(this RouteGroupBuilder group)
    {
        group.MapPost("/account/register", RegisterAsync)
            .AllowAnonymous()
            .Produces(StatusCodes.Status202Accepted)
            .ProducesValidationProblem();

        group.MapGet("/account/confirm-email", ConfirmEmailAsync)
            .AllowAnonymous();

        group.MapPost("/account/login", LoginAsync)
            .AllowAnonymous()
            .Produces<LoginResponse>()
            .Produces(StatusCodes.Status202Accepted)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return group;
    }

    private static async Task<IResult> RegisterAsync(
        RegisterRequest input,
        HttpRequest request,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IAccountEmailSender emailSender,
        CancellationToken cancellationToken)
    {
        var errors = Validate(input);
        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        var email = input.Email.Trim();
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = false
        };
        var result = await userManager.CreateAsync(user, input.Password);
        if (!result.Succeeded)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(input.Email)] = result.Errors.Select(error => error.Description).ToArray()
            });
        }

        if (!await roleManager.RoleExistsAsync(ApplicationRoles.Client))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Client));
            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(user);
                return IdentityErrors(nameof(ApplicationRoles.Client), roleResult.Errors);
            }
        }
        var addToRoleResult = await userManager.AddToRoleAsync(user, ApplicationRoles.Client);
        if (!addToRoleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return IdentityErrors(nameof(ApplicationRoles.Client), addToRoleResult.Errors);
        }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var confirmationUrl = $"{request.Scheme}://{request.Host}{request.PathBase}" +
            $"/api/auth/account/confirm-email?userId={Uri.EscapeDataString(user.Id)}" +
            $"&code={Uri.EscapeDataString(encodedToken)}";
        try
        {
            await emailSender.SendConfirmationAsync(user.Email!, confirmationUrl, cancellationToken);
        }
        catch
        {
            await userManager.DeleteAsync(user);
            throw;
        }

        return Results.Accepted(value: new
        {
            message = "Revisa tu correo para confirmar la cuenta."
        });
    }

    private static async Task<IResult> ConfirmEmailAsync(
        string userId,
        string code,
        AccountOptions account,
        UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Results.NotFound();
        }

        string token;
        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(code)] = ["El código de confirmación no es válido."]
            });
        }

        var result = await userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(code)] = ["El enlace venció o ya no es válido."]
            });
        }

        var loginUrl = string.IsNullOrWhiteSpace(account.WebLoginUrl)
            ? "/"
            : account.WebLoginUrl;
        return Results.Redirect($"{loginUrl}?confirmed=true");
    }

    private static async Task<IResult> LoginAsync(
        PasswordLoginRequest input,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        var email = input.Email?.Trim();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(input.Password))
        {
            return Results.Problem(
                title: "Credenciales inválidas",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
        {
            return Results.Problem(
                title: "Credenciales inválidas o correo sin confirmar",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        var result = await signInManager.PasswordSignInAsync(
            user,
            input.Password,
            isPersistent: true,
            lockoutOnFailure: true);
        if (result.RequiresTwoFactor)
        {
            return Results.Accepted(value: new { requiresMfa = true });
        }

        if (!result.Succeeded)
        {
            return Results.Problem(
                title: "Credenciales inválidas",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        var roles = await userManager.GetRolesAsync(user);
        return Results.Ok(new LoginResponse(
            string.Empty,
            "Cookie",
            DateTimeOffset.UtcNow.AddHours(8),
            user.Email ?? user.UserName ?? "cliente",
            roles.ToArray()));
    }

    private static Dictionary<string, string[]> Validate(RegisterRequest input)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(input.Email) ||
            !new EmailAddressAttribute().IsValid(input.Email))
        {
            errors[nameof(input.Email)] = ["Ingresa un correo válido."];
        }
        if (string.IsNullOrEmpty(input.Password) || input.Password.Length < 8)
        {
            errors[nameof(input.Password)] = ["La contraseña debe tener al menos 8 caracteres."];
        }
        if (!string.Equals(input.Password, input.ConfirmPassword, StringComparison.Ordinal))
        {
            errors[nameof(input.ConfirmPassword)] = ["Las contraseñas no coinciden."];
        }
        return errors;
    }

    private static IResult IdentityErrors(string key, IEnumerable<IdentityError> errors) =>
        Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [key] = errors.Select(error => error.Description).ToArray()
        });

    private sealed record RegisterRequest(string Email, string Password, string ConfirmPassword);
    private sealed record PasswordLoginRequest(string Email, string Password);
}
