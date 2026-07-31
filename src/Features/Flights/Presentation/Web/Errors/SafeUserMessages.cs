namespace Airport.Features.Flights.Presentation.Web.Errors;

public static class SafeUserMessages
{
    public const string Unauthorized =
        "No tienes autorización para realizar esta acción.";

    public const string ServiceUnavailable =
        "El servicio no está disponible en este momento. Inténtalo nuevamente.";

    public const string UnexpectedResponse =
        "No pudimos interpretar la respuesta del servicio.";
}
