namespace Airport.Features.Auth.Application.Security;

public static class TokenLifetimePolicy
{
    public const int RecommendedMinutes = 15;
    public const int MinimumMinutes = 5;
    public const int MaximumMinutes = 30;
}
