using Airport.Features.Administration.Application.GetDatabaseSummary;
using Microsoft.EntityFrameworkCore;

namespace Airport.Features.Administration.Infrastructure.Persistence;

public sealed class PostgresDatabaseSummaryReader(
    AdministrationDbContext dbContext,
    TimeProvider timeProvider) : IDatabaseSummaryReader
{
    private const string EstimatesSql = """
        SELECT c.relname AS "TableName",
               GREATEST(c.reltuples, 0)::bigint AS "EstimatedRows"
        FROM pg_class AS c
        INNER JOIN pg_namespace AS n ON n.oid = c.relnamespace
        WHERE n.nspname = 'airportdb'
          AND c.relkind = 'r'
          AND c.relname IN (
              'airline', 'airplane', 'airplane_type', 'airport', 'airport_geo',
              'airport_reachable', 'booking', 'employee', 'flight', 'flight_log',
              'flightschedule', 'passenger', 'passengerdetails', 'weatherdata')
        """;

    private static readonly IReadOnlyDictionary<string, string> Labels =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["flight"] = "Vuelos",
            ["booking"] = "Reservas",
            ["passenger"] = "Pasajeros",
            ["passengerdetails"] = "Detalles de pasajeros",
            ["employee"] = "Empleados",
            ["airplane"] = "Aeronaves",
            ["airplane_type"] = "Tipos de aeronave",
            ["airline"] = "Aerolíneas",
            ["airport"] = "Aeropuertos",
            ["airport_geo"] = "Ubicaciones de aeropuertos",
            ["airport_reachable"] = "Rutas entre aeropuertos",
            ["flightschedule"] = "Programación de vuelos",
            ["flight_log"] = "Historial de vuelos",
            ["weatherdata"] = "Registros meteorológicos"
        };

    public async Task<DatabaseSummaryResponse> ReadAsync(CancellationToken cancellationToken)
    {
        var estimates = await dbContext.Database
            .SqlQueryRaw<CatalogTableEstimate>(EstimatesSql)
            .ToListAsync(cancellationToken);
        var items = estimates
            .Where(estimate => Labels.ContainsKey(estimate.TableName))
            .Select(estimate => new DatabaseSummaryItem(
                Labels[estimate.TableName],
                estimate.EstimatedRows))
            .OrderByDescending(item => item.Count)
            .ToArray();

        return new DatabaseSummaryResponse(
            items.Sum(item => item.Count),
            true,
            timeProvider.GetUtcNow(),
            items);
    }
}
