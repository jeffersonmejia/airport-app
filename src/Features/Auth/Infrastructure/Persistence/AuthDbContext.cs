using Microsoft.EntityFrameworkCore;

namespace Airport.Features.Auth.Infrastructure.Persistence;

public sealed class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<AuthEmployeeRow> Employees => Set<AuthEmployeeRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthEmployeeRow>(entity =>
        {
            entity.ToTable("employee", "airportdb");
            entity.HasKey(employee => employee.EmployeeId);

            entity.Property(employee => employee.EmployeeId).HasColumnName("employee_id");
            entity.Property(employee => employee.Username)
                .HasColumnName("username")
                .HasMaxLength(20)
                .IsRequired();
            entity.Property(employee => employee.PasswordHash)
                .HasColumnName("password")
                .HasColumnType("character(32)")
                .IsRequired();
            entity.Property(employee => employee.Department)
                .HasColumnName("department")
                .HasColumnType("airportdb.employee_department")
                .IsRequired();
        });
    }
}
