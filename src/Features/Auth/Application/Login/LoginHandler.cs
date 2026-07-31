using Airport.Features.Auth.Application.Ports;
using Airport.Features.Auth.Application.Roles;
using Airport.Features.Auth.Domain;

namespace Airport.Features.Auth.Application.Login;

public sealed class LoginHandler(
    IEmployeeCredentialReader employees,
    IPasswordVerifier passwords,
    IAccessTokenIssuer tokens)
{
    private const string DummyPasswordHash = "00000000000000000000000000000000";

    public async Task<LoginResponse?> HandleAsync(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var employee = await employees.FindByUsernameAsync(
            command.Username.Trim(),
            cancellationToken);
        var passwordMatches = passwords.Verify(
            command.Password,
            employee?.PasswordHash ?? DummyPasswordHash);

        if (employee is null || !passwordMatches)
        {
            return null;
        }

        var roles = EmployeeDepartmentRoleMapper.Map(employee.Department);
        var identity = new AuthIdentity(employee.EmployeeId, employee.Username, roles);
        var accessToken = await tokens.IssueAsync(identity, cancellationToken);

        return new LoginResponse(
            accessToken.Token,
            "Bearer",
            accessToken.ExpiresAt,
            employee.Username,
            roles);
    }
}
