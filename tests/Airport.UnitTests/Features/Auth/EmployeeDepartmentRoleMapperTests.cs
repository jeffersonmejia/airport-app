using Airport.Features.Auth.Application.Roles;

namespace Airport.UnitTests.Auth;

public sealed class EmployeeDepartmentRoleMapperTests
{
    public static TheoryData<string, string[]> DepartmentRoles => new()
    {
        { "Marketing", [ApplicationRoles.Client] },
        { "Buchhaltung", [ApplicationRoles.Client] },
        { "Management", [ApplicationRoles.Client, ApplicationRoles.Admin] },
        { "Logistik", [ApplicationRoles.Client] },
        { "Flugfeld", [ApplicationRoles.Client] }
    };

    [Theory]
    [MemberData(nameof(DepartmentRoles))]
    public void MapReturnsExpectedRoles(string department, string[] expectedRoles)
    {
        var roles = EmployeeDepartmentRoleMapper.Map(department);

        Assert.Equal(expectedRoles, roles);
    }

    [Fact]
    public void MapRejectsUnknownDepartment()
    {
        Assert.Throws<InvalidOperationException>(
            () => EmployeeDepartmentRoleMapper.Map("Unknown"));
    }
}
