using Airport.Web;
using Airport.Features.Administration.Presentation.Web;
using Airport.Features.Auth.Presentation.Web;
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
builder.Services.AddAuthPresentation();
builder.Services.AddAdministrationPresentation();

await builder.Build().RunAsync();
