namespace Airport.Features.Auth.Application.Ports;

public sealed record EmployeeCredential(
    int EmployeeId,
    string Username,
    string PasswordHash,
    string Department);
