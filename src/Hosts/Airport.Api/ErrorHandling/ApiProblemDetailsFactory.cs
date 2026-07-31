using Microsoft.AspNetCore.Mvc;

namespace Airport.Api.ErrorHandling;

public static class ApiProblemDetailsFactory
{
    public static ProblemDetails Create(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => Build(
            statusCode,
            "Solicitud inválida",
            "La solicitud no pudo ser procesada."),
        StatusCodes.Status401Unauthorized => Build(
            statusCode,
            "No autorizado",
            "Debes autenticarte para acceder a este recurso."),
        StatusCodes.Status403Forbidden => Build(
            statusCode,
            "Acceso denegado",
            "No tienes permisos para acceder a este recurso."),
        StatusCodes.Status404NotFound => Build(
            statusCode,
            "Recurso no encontrado",
            "El recurso solicitado no existe."),
        StatusCodes.Status500InternalServerError => Build(
            statusCode,
            "Error interno del servidor",
            "No pudimos completar la solicitud. Inténtalo nuevamente."),
        _ => Build(
            statusCode,
            "Solicitud no procesada",
            "No pudimos completar la solicitud.")
    };

    private static ProblemDetails Build(int statusCode, string title, string detail) => new()
    {
        Status = statusCode,
        Title = title,
        Detail = detail
    };
}
