namespace Airport.Features.Auth.Application.Ports;

public interface IPasswordVerifier
{
    bool Verify(string password, string storedHash);
}
