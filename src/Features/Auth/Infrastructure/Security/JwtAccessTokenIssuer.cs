using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Airport.Features.Auth.Application.Ports;
using Airport.Features.Auth.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Airport.Features.Auth.Infrastructure.Security;

public sealed class JwtAccessTokenIssuer(
    IOptions<JwtOptions> options,
    IActiveSessionRegistry sessions,
    TimeProvider timeProvider) : IAccessTokenIssuer
{
    private readonly JwtOptions jwt = options.Value;

    public async ValueTask<IssuedAccessToken> IssueAsync(
        AuthIdentity identity,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var expiresAt = now.AddMinutes(jwt.AccessTokenMinutes);
        var sessionId = Guid.NewGuid().ToString("N");
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, identity.UserId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, identity.Username),
            new(JwtRegisteredClaimNames.Jti, sessionId),
            new(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };
        claims.AddRange(identity.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            jwt.Issuer,
            jwt.Audience,
            claims,
            now.UtcDateTime,
            expiresAt.UtcDateTime,
            credentials);

        await sessions.ActivateAsync(
            identity.UserId,
            sessionId,
            expiresAt - now,
            cancellationToken);

        return new IssuedAccessToken(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt,
            sessionId);
    }
}
