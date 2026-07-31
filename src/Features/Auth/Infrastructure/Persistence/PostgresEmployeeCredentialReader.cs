using Airport.Features.Auth.Application.Ports;
using Microsoft.EntityFrameworkCore;

namespace Airport.Features.Auth.Infrastructure.Persistence;

public sealed class PostgresEmployeeCredentialReader(AuthDbContext dbContext)
    : IEmployeeCredentialReader
{
    public Task<EmployeeCredential?> FindByUsernameAsync(
        string username,
        CancellationToken cancellationToken) =>
        dbContext.Employees
            .AsNoTracking()
            .Where(employee => employee.Username == username)
            .Select(employee => new EmployeeCredential(
                employee.EmployeeId,
                employee.Username,
                employee.PasswordHash.Trim(),
                employee.Department.ToString()))
            .SingleOrDefaultAsync(cancellationToken);
}
