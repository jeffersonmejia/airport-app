namespace Airport.Features.Auth.Application.Ports;

public interface IAccountEmailSender
{
    Task SendConfirmationAsync(
        string recipient,
        string confirmationUrl,
        CancellationToken cancellationToken);
}
