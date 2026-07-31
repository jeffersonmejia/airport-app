using Airport.Web;
using Airport.Features.Flights.Presentation.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"];

if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    throw new InvalidOperationException("Falta la configuración pública ApiBaseUrl.");
}

var apiBaseAddress = new Uri(
    new Uri(builder.HostEnvironment.BaseAddress),
    apiBaseUrl);

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = apiBaseAddress
});
builder.Services.AddFlightsPresentation();

await builder.Build().RunAsync();
