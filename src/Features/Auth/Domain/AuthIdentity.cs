namespace Airport.Features.Auth.Domain;

public sealed record AuthIdentity(
    int UserId,
    string Username,
    IReadOnlyCollection<string> Roles);
