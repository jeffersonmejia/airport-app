namespace Airport.Features.Auth.Infrastructure.Email;

public sealed class EmailOptions
{
    public const string SectionName = "EmailSettings";

    public string SmtpServer { get; init; } = string.Empty;
    public int Port { get; init; } = 587;
    public string SenderEmail { get; init; } = string.Empty;
    public string SenderName { get; init; } = "Airport";
    public string Password { get; init; } = string.Empty;
}
