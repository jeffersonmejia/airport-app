namespace Airport.Features.Payments.Application.CreatePayPalOrder;

public sealed class CreatePayPalOrderValidator
{
    public IReadOnlyDictionary<string, string[]> Validate(CreatePayPalOrderCommand command)
    {
        var errors = new Dictionary<string, string[]>();

        if (command.Amount <= 0 || command.Amount > 999_999.99m)
        {
            errors[nameof(command.Amount)] = ["El monto debe estar entre 0.01 y 999999.99."];
        }

        if (!string.Equals(command.CurrencyCode?.Trim(), "USD", StringComparison.OrdinalIgnoreCase))
        {
            errors[nameof(command.CurrencyCode)] = ["La moneda permitida es USD."];
        }

        if (string.IsNullOrWhiteSpace(command.ReferenceId) || command.ReferenceId.Trim().Length > 255)
        {
            errors[nameof(command.ReferenceId)] = ["La referencia es obligatoria y admite hasta 255 caracteres."];
        }

        if (string.IsNullOrWhiteSpace(command.Description) || command.Description.Trim().Length > 127)
        {
            errors[nameof(command.Description)] = ["La descripción es obligatoria y admite hasta 127 caracteres."];
        }

        if (string.IsNullOrWhiteSpace(command.IdempotencyKey) || command.IdempotencyKey.Trim().Length > 108)
        {
            errors[nameof(command.IdempotencyKey)] = ["PayPal-Request-Id es obligatorio y admite hasta 108 caracteres."];
        }

        return errors;
    }
}
