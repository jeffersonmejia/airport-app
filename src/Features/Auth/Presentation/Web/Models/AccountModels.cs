using System.ComponentModel.DataAnnotations;

namespace Airport.Features.Auth.Presentation.Web.Models;

public sealed record LoginAttemptViewModel(
    LoginResultViewModel? Session,
    bool RequiresMfa);

public sealed class RegisterInput
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required, Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;
}
