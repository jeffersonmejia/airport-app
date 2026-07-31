namespace Airport.Features.Auth.Application.Roles;

public static class EmployeeDepartmentRoleMapper
{
    public static IReadOnlyCollection<string> Map(string department) => department switch
    {
        "Marketing" => [ApplicationRoles.Marketing],
        "Buchhaltung" => [ApplicationRoles.Accounting],
        "Management" => [ApplicationRoles.Management, ApplicationRoles.Admin],
        "Logistik" => [ApplicationRoles.Logistics],
        "Flugfeld" => [ApplicationRoles.AirfieldOperations],
        _ => throw new InvalidOperationException(
            $"El departamento '{department}' no tiene roles configurados.")
    };
}
