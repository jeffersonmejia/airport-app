using System.Net.Http.Json;
using Airport.Features.Administration.Presentation.Web.Models;

namespace Airport.Features.Administration.Presentation.Web.Services;

public sealed class AdministrationClient(HttpClient httpClient)
{
    public async Task<DatabaseSummaryViewModel> GetDatabaseSummaryAsync(
        CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(
            "api/admin/database-summary",
            cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<DatabaseSummaryViewModel>(
            cancellationToken)
            ?? throw new InvalidOperationException("La API devolvió un resumen vacío.");
    }

    public async Task<CommerceOverviewViewModel> GetCommerceAsync(
        int page,
        CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync($"api/admin/commerce?page={page}", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CommerceOverviewViewModel>(cancellationToken)
            ?? throw new InvalidOperationException("La API devolvió operaciones vacías.");
    }
}
