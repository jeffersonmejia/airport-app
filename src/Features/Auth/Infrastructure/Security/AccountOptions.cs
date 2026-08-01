namespace Airport.Features.Auth.Infrastructure.Security;

public sealed class AccountOptions
{
    public const string SectionName = "Authentication:Account";

    public string WebLoginUrl { get; init; } = string.Empty;
}
