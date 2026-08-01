namespace Airport.Features.Auth.Application.Roles;

public static class EmployeeDepartmentRoleMapper
{
    public static IReadOnlyCollection<string> Map(string department) => department switch
    {
        "Management" => [ApplicationRoles.Client, ApplicationRoles.Admin],
        "Marketing" or "Buchhaltung" or "Logistik" or "Flugfeld" =>
            [ApplicationRoles.Client],
        _ => throw new InvalidOperationException(
            $"El departamento '{department}' no tiene roles configurados.")
    };
}
