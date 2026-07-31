namespace Airport.Features.Auth.Application.Login;

public sealed record LoginResponse(
    string AccessToken,
    string TokenType,
    DateTimeOffset ExpiresAt,
    string Username,
    IReadOnlyCollection<string> Roles);
