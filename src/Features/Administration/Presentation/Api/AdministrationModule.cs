using Airport.Features.Administration.Application.GetDatabaseSummary;
using Airport.Features.Administration.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Airport.Features.Administration.Presentation.Api;

public static class AdministrationModule
{
    public static IServiceCollection AddAdministrationModule(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddScoped<GetDatabaseSummaryHandler>();
        services.AddAdministrationInfrastructure(connectionString);
        return services;
    }

    public static IEndpointRouteBuilder MapAdministrationModule(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/admin/database-summary", HandleAsync)
            .RequireAuthorization("AdminOnly")
            .WithName("GetDatabaseSummary")
            .WithSummary("Obtiene el resumen aproximado de registros para administración")
            .Produces<DatabaseSummaryResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        return endpoints;
    }

    private static async Task<IResult> HandleAsync(
        GetDatabaseSummaryHandler handler,
        CancellationToken cancellationToken) =>
        Results.Ok(await handler.HandleAsync(cancellationToken));
}
