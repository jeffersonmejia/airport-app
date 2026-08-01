using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Airport.Features.Auth.Application.Ports;
using Airport.Features.Auth.Application.Login;
using Airport.Features.Auth.Application.Roles;
using Airport.Features.Auth.Infrastructure;
using Airport.Features.Auth.Infrastructure.Security;
using Airport.Features.Auth.Presentation.Api.Login;
using Airport.Features.Auth.Presentation.Api.Google;
using Airport.Features.Auth.Presentation.Api.Mfa;
using Airport.Features.Auth.Presentation.Api.Session;
using Airport.Features.Auth.Presentation.Api.Account;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Airport.Features.Auth.Presentation.Api;

public static class AuthModule
{
    public static IServiceCollection AddAuthModule(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionString)
    {
        var jwt = configuration.GetRequiredSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException("Falta la configuración JWT en User Secrets.");

        services.AddSingleton<LoginValidator>();
        services.AddScoped<LoginHandler>();
        services.AddAuthInfrastructure(configuration, connectionString);

        var google = configuration.GetSection(GoogleAuthOptions.SectionName)
            .Get<GoogleAuthOptions>() ?? new GoogleAuthOptions();
        services.AddSingleton(google);
        var account = configuration.GetSection(AccountOptions.SectionName)
            .Get<AccountOptions>() ?? new AccountOptions();
        services.AddSingleton(account);

        var authentication = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "AirportAuth";
            options.DefaultChallengeScheme = "AirportAuth";
        });

        authentication.AddPolicyScheme("AirportAuth", "JWT o cookie de Identity", options =>
        {
            options.ForwardDefaultSelector = context =>
                context.Request.Headers.Authorization.ToString()
                    .StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                    ? JwtBearerDefaults.AuthenticationScheme
                    : IdentityConstants.ApplicationScheme;
        });
        authentication.AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwt.SigningKey)),
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    NameClaimType = JwtRegisteredClaimNames.UniqueName,
                    RoleClaimType = ClaimTypes.Role,
                    ClockSkew = TimeSpan.FromSeconds(jwt.ClockSkewSeconds)
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = ValidateActiveSessionAsync
                };
            });
        authentication.AddIdentityCookies();

        services.Configure<CookieAuthenticationOptions>(
            IdentityConstants.ApplicationScheme,
            options =>
            {
                options.Cookie.Name = "Airport.Identity";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });

        if (google.IsConfigured)
        {
            authentication.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = google.ClientId;
                options.ClientSecret = google.ClientSecret;
                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.SaveTokens = false;
            });
        }
        services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", policy => policy.RequireRole(ApplicationRoles.Admin))
            .AddPolicy("ClientOnly", policy => policy.RequireRole(ApplicationRoles.Client));

        return services;
    }

    public static IEndpointRouteBuilder MapAuthModule(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGroup("/api/auth")
            .MapLogin()
            .MapAccount()
            .MapGoogleAuth()
            .MapMfa()
            .MapSession();

        return endpoints;
    }

    private static async Task ValidateActiveSessionAsync(TokenValidatedContext context)
    {
        var subject = context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var sessionId = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

        if (!int.TryParse(subject, out var userId) || string.IsNullOrWhiteSpace(sessionId))
        {
            context.Fail("El token no contiene una sesión válida.");
            return;
        }

        var registry = context.HttpContext.RequestServices
            .GetRequiredService<IActiveSessionRegistry>();
        var isActive = await registry.IsActiveAsync(
            userId,
            sessionId,
            context.HttpContext.RequestAborted);

        if (!isActive)
        {
            context.Fail("La sesión fue reemplazada o cerrada.");
        }
    }
}
