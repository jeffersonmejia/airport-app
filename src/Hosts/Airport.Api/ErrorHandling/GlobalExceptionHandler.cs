using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Api.ErrorHandling;

public sealed partial class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        LogUnhandledException(logger, httpContext.TraceIdentifier, exception);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var problemDetails = ApiProblemDetailsFactory.Create(httpContext.Response.StatusCode);
        var written = await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });

        if (!written)
        {
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        }

        return true;
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Unhandled API exception. Trace identifier: {TraceIdentifier}")]
    private static partial void LogUnhandledException(
        ILogger logger,
        string traceIdentifier,
        Exception exception);
}
