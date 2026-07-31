using System.Net.Http.Json;
using Airport.Features.Administration.Presentation.Web.Models;

namespace Airport.Features.Administration.Presentation.Web.Services;

public sealed class AdministrationClient(HttpClient httpClient)
{
    public async Task<DatabaseSummaryViewModel> GetDatabaseSummaryAsync(
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(
            "api/admin/database-summary",
            cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<DatabaseSummaryViewModel>(
            cancellationToken)
            ?? throw new InvalidOperationException("La API devolvió un resumen vacío.");
    }
}
