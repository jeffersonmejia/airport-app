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
        var safeConfirmationUrl = System.Net.WebUtility.HtmlEncode(confirmationUrl);
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(settings.SenderName, settings.SenderEmail));
        message.To.Add(MailboxAddress.Parse(recipient));
        message.Subject = "Confirma tu correo y empieza a viajar con Airport";
        message.Body = new BodyBuilder
        {
            HtmlBody = $"""
                <!doctype html>
                <html lang="es">
                <head>
                    <meta charset="utf-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1">
                    <title>Confirma tu cuenta de Airport</title>
                </head>
                <body style="margin:0;padding:0;background:#fff7f9;color:#24191d;font-family:Arial,Helvetica,sans-serif;">
                    <div style="display:none;max-height:0;overflow:hidden;opacity:0;">
                        Confirma tu correo para guardar órdenes, boletos y comprobantes en Airport.
                    </div>
                    <table role="presentation" width="100%" cellspacing="0" cellpadding="0"
                           style="width:100%;background:#fff7f9;border-collapse:collapse;">
                        <tr>
                            <td align="center" style="padding:32px 12px;">
                                <table role="presentation" width="100%" cellspacing="0" cellpadding="0"
                                       style="width:100%;max-width:600px;background:#ffffff;border:1px solid #dec6ce;border-radius:28px;border-collapse:separate;overflow:hidden;box-shadow:0 18px 50px rgba(84,43,58,.10);">
                                    <tr>
                                        <td style="padding:28px 32px 20px;text-align:center;background:linear-gradient(145deg,#ffe8ef,#f9bfd1);">
                                            <div style="display:inline-block;width:56px;height:56px;line-height:56px;background:#e85d8e;border-radius:18px;color:#ffffff;font-size:28px;text-align:center;box-shadow:0 10px 22px rgba(232,93,142,.26);">
                                                &#9992;
                                            </div>
                                            <p style="margin:14px 0 0;color:#923051;font-size:13px;font-weight:700;letter-spacing:1.2px;text-transform:uppercase;">
                                                Airport
                                            </p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding:32px;">
                                            <p style="margin:0 0 8px;color:#7c5360;font-size:13px;font-weight:700;letter-spacing:.8px;text-transform:uppercase;">
                                                Cuenta de cliente
                                            </p>
                                            <h1 style="margin:0 0 14px;color:#24191d;font-size:30px;line-height:1.2;">
                                                Confirma tu correo
                                            </h1>
                                            <p style="margin:0 0 26px;color:#6b5860;font-size:16px;line-height:1.65;">
                                                Estás a un paso de completar tu registro. Confirma tu dirección para buscar vuelos, comprar boletos y consultar tus comprobantes de forma segura.
                                            </p>

                                            <table role="presentation" cellspacing="0" cellpadding="0" style="margin:0 auto 28px;">
                                                <tr>
                                                    <td align="center" bgcolor="#e85d8e" style="border-radius:12px;box-shadow:0 8px 20px rgba(232,93,142,.22);">
                                                        <a href="{safeConfirmationUrl}"
                                                           style="display:inline-block;padding:16px 28px;color:#ffffff;font-size:16px;font-weight:700;text-decoration:none;border-radius:12px;">
                                                            Confirmar mi correo
                                                        </a>
                                                    </td>
                                                </tr>
                                            </table>

                                            <table role="presentation" width="100%" cellspacing="0" cellpadding="0"
                                                   style="width:100%;background:#fff2f6;border-radius:16px;border-collapse:separate;">
                                                <tr>
                                                    <td style="padding:18px 20px;color:#6b5860;font-size:14px;line-height:1.55;">
                                                        <strong style="color:#923051;">Tu seguridad es importante</strong><br>
                                                        Este enlace es personal. Si no creaste una cuenta en Airport, puedes ignorar este mensaje.
                                                    </td>
                                                </tr>
                                            </table>

                                            <p style="margin:26px 0 8px;color:#6b5860;font-size:12px;line-height:1.5;">
                                                Si el botón no funciona, copia y pega este enlace en tu navegador:
                                            </p>
                                            <p style="margin:0;word-break:break-all;font-size:12px;line-height:1.5;">
                                                <a href="{safeConfirmationUrl}" style="color:#923051;text-decoration:underline;">
                                                    {safeConfirmationUrl}
                                                </a>
                                            </p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding:20px 32px;background:#f7e8ed;color:#7c5360;font-size:12px;line-height:1.5;text-align:center;border-radius:0 0 28px 28px;">
                                            Este mensaje fue enviado automáticamente por Airport.<br>
                                            No respondas a este correo.
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>
                """,
            TextBody = $"""
                AIRPORT

                Confirma tu correo

                Estás a un paso de completar tu registro. Confirma tu dirección para buscar vuelos,
                comprar boletos y consultar tus comprobantes de forma segura.

                Confirmar mi correo:
                {confirmationUrl}

                Este enlace es personal. Si no creaste una cuenta en Airport, ignora este mensaje.

                Este mensaje fue enviado automáticamente. No respondas a este correo.
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
