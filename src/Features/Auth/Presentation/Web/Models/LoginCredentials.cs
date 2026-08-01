using System.ComponentModel.DataAnnotations;

namespace Airport.Features.Auth.Presentation.Web.Models;

public sealed class LoginCredentials
{
    [Required(ErrorMessage = "Ingresa tu usuario.")]
    [MaxLength(60, ErrorMessage = "El usuario no puede superar 60 caracteres.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa tu contraseña.")]
    [MaxLength(100, ErrorMessage = "La contraseña no puede superar 100 caracteres.")]
    public string Password { get; set; } = string.Empty;
}
