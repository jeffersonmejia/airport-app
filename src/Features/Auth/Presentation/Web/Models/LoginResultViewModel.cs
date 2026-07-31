namespace Airport.Features.Auth.Presentation.Web.Models;

public sealed record LoginResultViewModel(
    string AccessToken,
    string TokenType,
    DateTimeOffset ExpiresAt,
    string Username,
    IReadOnlyCollection<string> Roles);
