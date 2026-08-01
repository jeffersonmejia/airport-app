using Airport.Features.Auth.Application.Ports;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Airport.Features.Auth.Infrastructure.Email;

internal sealed class SmtpAccountEmailSender(IOptions<EmailOptions> options) : IAccountEmailSender
{
    private readonly EmailOptions settings = options.Value;

    public async Task SendConfirmationAsync(
        string recipient,
        string confirmationUrl,
        CancellationToken cancellationToken)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(settings.SenderName, settings.SenderEmail));
        message.To.Add(MailboxAddress.Parse(recipient));
        message.Subject = "Confirma tu cuenta de Airport";
        message.Body = new BodyBuilder
        {
            HtmlBody = $"""
                <h1>Confirma tu cuenta</h1>
                <p>Termina tu registro para comprar boletos en Airport.</p>
                <p><a href="{System.Net.WebUtility.HtmlEncode(confirmationUrl)}">Confirmar correo</a></p>
                <p>Si no creaste esta cuenta, ignora este mensaje.</p>
                """
        }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(
            settings.SmtpServer,
            settings.Port,
            SecureSocketOptions.StartTlsWhenAvailable,
            cancellationToken);
        await client.AuthenticateAsync(settings.SenderEmail, settings.Password, cancellationToken);
        await client.SendAsync(message, cancellationToken, null);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
