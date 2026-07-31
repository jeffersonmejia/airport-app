namespace Airport.Features.Auth.Application.Ports;

public interface IEmployeeCredentialReader
{
    Task<EmployeeCredential?> FindByUsernameAsync(
        string username,
        CancellationToken cancellationToken);
}
