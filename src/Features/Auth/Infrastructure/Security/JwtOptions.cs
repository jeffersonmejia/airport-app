namespace Airport.Features.Auth.Infrastructure.Security;

public sealed class JwtOptions
{
    public const string SectionName = "Auth:Jwt";

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public string SigningKey { get; init; } = string.Empty;

    public int MinimumAccessTokenMinutes { get; init; }

    public int MaximumAccessTokenMinutes { get; init; }

    public int AccessTokenMinutes { get; init; }

    public int ClockSkewSeconds { get; init; }

    public bool HasValidAccessTokenRange =>
        MinimumAccessTokenMinutes > 0
        && MaximumAccessTokenMinutes >= MinimumAccessTokenMinutes;

    public bool IsAccessTokenLifetimeAllowed =>
        HasValidAccessTokenRange
        && AccessTokenMinutes >= MinimumAccessTokenMinutes
        && AccessTokenMinutes <= MaximumAccessTokenMinutes;
}
