using System.Net;
using System.Net.Http.Json;
using Airport.Features.Flights.Presentation.Web.Models;

namespace Airport.Features.Flights.Presentation.Web.Services;

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

    public async Task<FlightSearchResultViewModel> SearchAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var result = await httpClient.GetFromJsonAsync<FlightSearchResultViewModel>(
            $"api/flights?page={page}&pageSize={pageSize}",
            cancellationToken);

        return result ?? throw new InvalidOperationException(
            "La API devolvió una respuesta vacía al buscar vuelos.");
    }
}
