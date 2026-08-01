using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Airport.Features.Auth.Application.Ports;
using Airport.Features.Auth.Application.Login;
using Airport.Features.Auth.Application.Roles;
using Airport.Features.Auth.Infrastructure;
using Airport.Features.Auth.Infrastructure.Security;
using Airport.Features.Auth.Presentation.Api.Login;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
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
        services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", policy => policy.RequireRole(ApplicationRoles.Admin));

        return services;
    }

    public static IEndpointRouteBuilder MapAuthModule(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGroup("/api/auth")
            .MapLogin();

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
