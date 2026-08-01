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
        FlightSearchInput input,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string?>
        {
            ["originCode"] = input.OriginCode,
            ["destinationCode"] = input.DestinationCode,
            ["departureDate"] = input.DepartureDate?.ToString("yyyy-MM-dd"),
            ["number"] = input.Number,
            ["airlineId"] = input.AirlineId?.ToString(),
            ["airplaneId"] = input.AirplaneId?.ToString(),
            ["sortBy"] = input.SortBy,
            ["descending"] = input.Descending.ToString().ToLowerInvariant(),
            ["page"] = page.ToString(),
            ["pageSize"] = pageSize.ToString()
        };
        var query = string.Join('&', parameters
            .Where(pair => !string.IsNullOrWhiteSpace(pair.Value))
            .Select(pair => $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value!)}"));
        var result = await httpClient.GetFromJsonAsync<FlightSearchResultViewModel>(
            $"api/flights?{query}",
            cancellationToken);

        return result ?? throw new InvalidOperationException(
            "La API devolvió una respuesta vacía al buscar vuelos.");
    }
}
