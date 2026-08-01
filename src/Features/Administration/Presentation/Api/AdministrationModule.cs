using Airport.Features.Administration.Application.GetDatabaseSummary;
using Airport.Features.Administration.Application.GetCommerceOverview;
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
        services.AddScoped<GetCommerceOverviewHandler>();
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

        endpoints.MapGet("/api/admin/commerce", HandleCommerceAsync)
            .RequireAuthorization("AdminOnly")
            .WithName("GetCommerceOverview")
            .WithSummary("Lista órdenes, pagos, transacciones y boletos")
            .Produces<CommerceOverviewResponse>();

        return endpoints;
    }

    private static async Task<IResult> HandleAsync(
        GetDatabaseSummaryHandler handler,
        CancellationToken cancellationToken) =>
        Results.Ok(await handler.HandleAsync(cancellationToken));

    private static async Task<IResult> HandleCommerceAsync(
        int? page,
        GetCommerceOverviewHandler handler,
        CancellationToken cancellationToken)
    {
        var selectedPage = page ?? 1;
        if (selectedPage < 1) return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [nameof(page)] = ["La página debe ser mayor que cero."]
        });
        return Results.Ok(await handler.HandleAsync(selectedPage, 5, cancellationToken));
    }
}
