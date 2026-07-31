using System.Globalization;
using System.Net.Http.Json;
using Airport.Features.Bookings.Presentation.Web.Models;

namespace Airport.Features.Bookings.Presentation.Web.Services;

public sealed class BookingsClient(HttpClient httpClient)
{
    public async Task<BookingSearchViewModel> SearchAsync(
        int? bookingId,
        int? flightId,
        int? passengerId,
        int page,
        CancellationToken cancellationToken)
    {
        var query = new List<string> { $"page={page}", "pageSize=5" };
        AddFilter(query, "bookingId", bookingId);
        AddFilter(query, "flightId", flightId);
        AddFilter(query, "passengerId", passengerId);

        using var response = await httpClient.GetAsync(
            $"api/bookings?{string.Join('&', query)}",
            cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<BookingSearchViewModel>(
            cancellationToken)
            ?? throw new InvalidOperationException("La API devolvió una página vacía.");
    }

    public async Task<BookingViewModel> CreateAsync(
        CreateBookingInput input,
        CancellationToken cancellationToken)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/bookings",
            new
            {
                flightId = input.FlightId,
                passengerId = input.PassengerId,
                input.Seat,
                input.Price
            },
            cancellationToken);
        return await ReadBookingAsync(response, cancellationToken);
    }

    public async Task<BookingViewModel> UpdateAsync(
        int bookingId,
        UpdateBookingInput input,
        CancellationToken cancellationToken)
    {
        using var response = await httpClient.PatchAsJsonAsync(
            $"api/bookings/{bookingId}",
            new { input.Seat, input.Price, input.Version },
            cancellationToken);
        return await ReadBookingAsync(response, cancellationToken);
    }

    public async Task<BookingViewModel> CancelAsync(
        int bookingId,
        string reason,
        CancellationToken cancellationToken)
    {
        using var response = await httpClient.PostAsJsonAsync(
            $"api/bookings/{bookingId}/cancel",
            new { reason },
            cancellationToken);
        return await ReadBookingAsync(response, cancellationToken);
    }

    private static void AddFilter(List<string> query, string name, int? value)
    {
        if (value is not null)
        {
            query.Add($"{name}={value.Value.ToString(CultureInfo.InvariantCulture)}");
        }
    }

    private static async Task<BookingViewModel> ReadBookingAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<BookingViewModel>(cancellationToken)
            ?? throw new InvalidOperationException("La API devolvió una reserva vacía.");
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode) return;

        var problem = await response.Content.ReadFromJsonAsync<ProblemViewModel>(cancellationToken);
        throw new BookingsClientException(
            response.StatusCode,
            problem?.Detail ?? "No fue posible completar la operación.");
    }

    private sealed record ProblemViewModel(string? Detail);
}
