namespace Airport.Features.Auth.Presentation.Web.Models;

public sealed record MfaSetupViewModel(
    string SharedKey,
    string AuthenticatorUri,
    string QrCodeDataUri,
    bool IsEnabled);

public sealed record EnableMfaResult(IReadOnlyCollection<string> RecoveryCodes);

public sealed record EnableMfaAttempt(
    EnableMfaResult? Result,
    string? ErrorMessage);

public sealed class MfaCodeInput
{
    public string Code { get; set; } = string.Empty;
}
