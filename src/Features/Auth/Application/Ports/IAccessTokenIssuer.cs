using Airport.Features.Auth.Domain;

namespace Airport.Features.Auth.Application.Ports;

public interface IAccessTokenIssuer
{
    ValueTask<IssuedAccessToken> IssueAsync(
        AuthIdentity identity,
        CancellationToken cancellationToken);
}
