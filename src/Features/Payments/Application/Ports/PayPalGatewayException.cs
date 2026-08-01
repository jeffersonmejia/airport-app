namespace Airport.Features.Payments.Application.Ports;

public sealed class PayPalGatewayException(
    string message,
    int? statusCode = null,
    string? debugId = null,
    Exception? innerException = null) : Exception(message, innerException)
{
    public int? StatusCode { get; } = statusCode;

    public string? DebugId { get; } = debugId;
}
