namespace Airport.Features.Bookings.Application.CreateOrder;

public sealed class CreateOrderValidator
{
    public IReadOnlyDictionary<string, string[]> Validate(CreateOrderCommand command)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(command.UserId)) errors[nameof(command.UserId)] = ["La sesión no es válida."];
        if (command.FlightId <= 0) errors[nameof(command.FlightId)] = ["Selecciona un vuelo válido."];
        if (string.IsNullOrWhiteSpace(command.FareCode)) errors[nameof(command.FareCode)] = ["Selecciona una tarifa."];
        if ((command.PassengerFirstName?.Trim().Length ?? 0) is < 2 or > 100) errors[nameof(command.PassengerFirstName)] = ["Ingresa el nombre del pasajero."];
        if ((command.PassengerLastName?.Trim().Length ?? 0) is < 2 or > 100) errors[nameof(command.PassengerLastName)] = ["Ingresa el apellido del pasajero."];
        var passport = command.PassportNumber?.Trim() ?? string.Empty;
        if (passport.Length is < 6 or > 20 || !passport.All(char.IsLetterOrDigit)) errors[nameof(command.PassportNumber)] = ["El pasaporte debe tener entre 6 y 20 caracteres alfanuméricos."];
        return errors;
    }
}
