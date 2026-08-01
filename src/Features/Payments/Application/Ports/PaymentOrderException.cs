namespace Airport.Features.Payments.Application.Ports;

public sealed class PaymentOrderException(string message) : Exception(message);
