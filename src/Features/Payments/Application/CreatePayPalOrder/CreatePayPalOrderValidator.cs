namespace Airport.Features.Payments.Application.CreatePayPalOrder;

public sealed class CreatePayPalOrderValidator
{
    public IReadOnlyDictionary<string, string[]> Validate(CreatePayPalOrderCommand command)
    {
        var errors = new Dictionary<string, string[]>();

        if (command.TicketOrderId == Guid.Empty)
        {
            errors[nameof(command.TicketOrderId)] = ["La orden de compra no es válida."];
        }

        if (string.IsNullOrWhiteSpace(command.UserId))
        {
            errors[nameof(command.UserId)] = ["La sesión no es válida."];
        }

        if (string.IsNullOrWhiteSpace(command.IdempotencyKey) || command.IdempotencyKey.Trim().Length > 108)
        {
            errors[nameof(command.IdempotencyKey)] = ["PayPal-Request-Id es obligatorio y admite hasta 108 caracteres."];
        }

        return errors;
    }
}
