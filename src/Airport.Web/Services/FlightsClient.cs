using System.Net;
using System.Net.Http.Json;
using Airport.Web.Models;

namespace Airport.Web.Services;

public sealed class FlightsClient(HttpClient httpClient)
{
    public async Task<FlightViewModel?> FindByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(
            $"api/flights/{id}",
            cancellationToken);

        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FlightViewModel>(
            cancellationToken);
    }
}
