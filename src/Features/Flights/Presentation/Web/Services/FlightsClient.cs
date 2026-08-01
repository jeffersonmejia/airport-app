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

    public async Task<IReadOnlyCollection<AirportViewModel>> ListAirportsAsync(
        int? originAirportId,
        CancellationToken cancellationToken)
    {
        var path = originAirportId is null
            ? "api/flights/airports"
            : $"api/flights/airports?originAirportId={originAirportId.Value}";

        return await httpClient.GetFromJsonAsync<IReadOnlyCollection<AirportViewModel>>(
            path,
            cancellationToken) ?? [];
    }

    public async Task<IReadOnlyCollection<DateOnly>> ListDepartureDatesAsync(
        int originAirportId,
        int destinationAirportId,
        CancellationToken cancellationToken) =>
        await httpClient.GetFromJsonAsync<IReadOnlyCollection<DateOnly>>(
            $"api/flights/dates?originAirportId={originAirportId}&destinationAirportId={destinationAirportId}",
            cancellationToken) ?? [];

    public async Task<IReadOnlyCollection<AirlineOptionViewModel>> ListAirlinesAsync(
        int originAirportId,
        int destinationAirportId,
        DateOnly departureDate,
        CancellationToken cancellationToken) =>
        await httpClient.GetFromJsonAsync<IReadOnlyCollection<AirlineOptionViewModel>>(
            $"api/flights/airlines?originAirportId={originAirportId}" +
            $"&destinationAirportId={destinationAirportId}&departureDate={departureDate:yyyy-MM-dd}",
            cancellationToken) ?? [];

    public async Task<IReadOnlyCollection<AirplaneOptionViewModel>> ListAirplanesAsync(
        short airlineId,
        int originAirportId,
        int destinationAirportId,
        DateOnly departureDate,
        CancellationToken cancellationToken) =>
        await httpClient.GetFromJsonAsync<IReadOnlyCollection<AirplaneOptionViewModel>>(
            $"api/flights/airplanes?airlineId={airlineId}&originAirportId={originAirportId}" +
            $"&destinationAirportId={destinationAirportId}&departureDate={departureDate:yyyy-MM-dd}",
            cancellationToken) ?? [];

    public async Task<IReadOnlyCollection<string>> ListFlightNumbersAsync(
        int originAirportId,
        int destinationAirportId,
        DateOnly? departureDate,
        short? airlineId,
        int? airplaneId,
        CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string?>
        {
            ["originAirportId"] = originAirportId.ToString(),
            ["destinationAirportId"] = destinationAirportId.ToString(),
            ["departureDate"] = departureDate?.ToString("yyyy-MM-dd"),
            ["airlineId"] = airlineId?.ToString(),
            ["airplaneId"] = airplaneId?.ToString()
        };
        var query = string.Join('&', parameters
            .Where(pair => pair.Value is not null)
            .Select(pair => $"{pair.Key}={Uri.EscapeDataString(pair.Value!)}"));

        return await httpClient.GetFromJsonAsync<IReadOnlyCollection<string>>(
            $"api/flights/numbers?{query}",
            cancellationToken) ?? [];
    }

    public async Task<FlightSearchResultViewModel> SearchAsync(
        FlightSearchInput input,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string?>
        {
            ["originAirportId"] = input.OriginAirportId?.ToString(),
            ["destinationAirportId"] = input.DestinationAirportId?.ToString(),
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
