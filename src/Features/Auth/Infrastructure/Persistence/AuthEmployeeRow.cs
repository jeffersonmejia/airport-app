namespace Airport.Features.Auth.Infrastructure.Persistence;

public sealed class AuthEmployeeRow
{
    public int EmployeeId { get; init; }
    public string Username { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public EmployeeDepartment Department { get; init; }
}
