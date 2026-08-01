using System.Net.Http.Json;
using Airport.Features.Bookings.Presentation.Web.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Airport.Features.Bookings.Presentation.Web.Services;

public sealed class BookingsClient(HttpClient httpClient)
{
    public async Task<OrderViewModel> CreateOrderAsync(
        int flightId,
        string fareCode,
        CheckoutInput input,
        CancellationToken cancellationToken)
    {
        using var request = CreateRequest(HttpMethod.Post, "api/bookings/orders");
        request.Content = JsonContent.Create(new
        {
            flightId,
            fareCode,
            input.PassengerFirstName,
            input.PassengerLastName,
            input.PassportNumber
        });
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderViewModel>(cancellationToken)
            ?? throw new InvalidOperationException("La API devolvió una orden vacía.");
    }

    public async Task<BookingHistoryViewModel> GetHistoryAsync(
        int page,
        CancellationToken cancellationToken)
    {
        using var request = CreateRequest(HttpMethod.Get, $"api/bookings/history?page={page}&pageSize=5");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BookingHistoryViewModel>(cancellationToken)
            ?? throw new InvalidOperationException("La API devolvió un historial vacío.");
    }

    public async Task<ReceiptViewModel?> GetReceiptAsync(Guid orderId, CancellationToken cancellationToken)
    {
        using var request = CreateRequest(HttpMethod.Get, $"api/bookings/orders/{orderId}/receipt");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ReceiptViewModel>(cancellationToken);
    }

    private static HttpRequestMessage CreateRequest(HttpMethod method, string uri)
    {
        var request = new HttpRequestMessage(method, uri);
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        return request;
    }
}
