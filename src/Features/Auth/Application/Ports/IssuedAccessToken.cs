namespace Airport.Features.Auth.Application.Ports;

public sealed record IssuedAccessToken(
    string Token,
    DateTimeOffset ExpiresAt,
    string SessionId);
