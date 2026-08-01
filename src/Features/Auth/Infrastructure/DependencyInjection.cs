using Airport.Features.Auth.Application.Ports;
using Airport.Features.Auth.Infrastructure.Persistence;
using Airport.Features.Auth.Infrastructure.Security;
using Airport.Features.Auth.Infrastructure.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Airport.Features.Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionString)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer), "JWT issuer is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience), "JWT audience is required.")
            .Validate(options => options.SigningKey.Length >= 32, "JWT signing key must have at least 32 characters.")
            .Validate(options => options.HasValidAccessTokenRange,
                "JWT lifetime limits must define a positive, ordered range.")
            .Validate(options => options.IsAccessTokenLifetimeAllowed,
                "JWT access token lifetime must be within the configured limits.")
            .Validate(options => options.ClockSkewSeconds is >= 0 and <= 120,
                "JWT clock skew must be between 0 and 120 seconds.")
            .ValidateOnStart();

        services.AddOptions<EmailOptions>()
            .Bind(configuration.GetSection(EmailOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.SmtpServer), "SMTP server is required.")
            .Validate(options => options.Port is > 0 and <= 65535, "SMTP port is invalid.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.SenderEmail), "SMTP sender is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Password), "SMTP password is required.")
            .ValidateOnStart();

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IActiveSessionRegistry, MemoryActiveSessionRegistry>();
        services.AddSingleton<IAccessTokenIssuer, JwtAccessTokenIssuer>();
        services.AddSingleton<IPasswordVerifier, LegacyMd5PasswordVerifier>();
        services.AddDbContextPool<AuthDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MapEnum<EmployeeDepartment>("employee_department", "airportdb")));
        services.AddDbContextPool<IdentityAuthDbContext>(options =>
            options.UseNpgsql(connectionString));
        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<IdentityAuthDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();
        services.AddScoped<IEmployeeCredentialReader, PostgresEmployeeCredentialReader>();
        services.AddScoped<IAccountEmailSender, SmtpAccountEmailSender>();

        return services;
    }
}
