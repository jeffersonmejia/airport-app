using Microsoft.AspNetCore.Mvc;

namespace Airport.Api.ErrorHandling;

public static class ErrorHandlingExtensions
{
    public static IServiceCollection AddApiErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Type = null;
                context.ProblemDetails.Instance = null;
                context.ProblemDetails.Extensions.Clear();
            };
        });
        services.AddExceptionHandler<GlobalExceptionHandler>();
        return services;
    }

    public static WebApplication UseApiErrorHandling(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseStatusCodePages(async statusCodeContext =>
        {
            var httpContext = statusCodeContext.HttpContext;
            var problemDetails = ApiProblemDetailsFactory.Create(httpContext.Response.StatusCode);
            var service = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
            var written = await service.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });

            if (!written)
            {
                await httpContext.Response.WriteAsJsonAsync(
                    problemDetails,
                    httpContext.RequestAborted);
            }
        });

        return app;
    }
}
