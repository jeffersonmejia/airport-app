namespace Airport.Features.Payments.Application.CapturePayPalOrder;

public sealed class CapturePayPalOrderValidator
{
    public IReadOnlyDictionary<string, string[]> Validate(CapturePayPalOrderCommand command)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(command.OrderId) ||
            command.OrderId.Trim().Length > 64 ||
            command.OrderId.Any(character => !char.IsLetterOrDigit(character)))
        {
            errors[nameof(command.OrderId)] = ["El identificador de la orden PayPal no es válido."];
        }

        if (string.IsNullOrWhiteSpace(command.IdempotencyKey) || command.IdempotencyKey.Trim().Length > 108)
        {
            errors[nameof(command.IdempotencyKey)] = ["PayPal-Request-Id es obligatorio y admite hasta 108 caracteres."];
        }

        if (string.IsNullOrWhiteSpace(command.UserId))
        {
            errors[nameof(command.UserId)] = ["La sesión no es válida."];
        }

        return errors;
    }
}
