namespace Airport.Features.Auth.Application.Login;

public sealed class LoginValidator
{
    public IReadOnlyDictionary<string, string[]> Validate(LoginCommand command)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(command.Username))
        {
            errors[nameof(command.Username)] = ["El nombre de usuario es obligatorio."];
        }
        else if (command.Username.Length > 60)
        {
            errors[nameof(command.Username)] = ["El nombre de usuario no puede superar 60 caracteres."];
        }

        if (string.IsNullOrEmpty(command.Password))
        {
            errors[nameof(command.Password)] = ["La contraseña es obligatoria."];
        }
        else if (command.Password.Length > 100)
        {
            errors[nameof(command.Password)] = ["La contraseña no puede superar 100 caracteres."];
        }

        return errors;
    }
}
