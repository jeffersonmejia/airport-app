using System.Net;

namespace Airport.Features.Bookings.Presentation.Web.Services;

public sealed class BookingsClientException(HttpStatusCode statusCode, string message)
    : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
