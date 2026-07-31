using Airport.Caching;
using Airport.Api.ErrorHandling;
using Airport.Features.Auth.Presentation.Api;
using Airport.Features.Flights.Presentation.Api;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AirportDb")
    ?? throw new InvalidOperationException(
        "Falta el secret ConnectionStrings:AirportDb. Configúralo con dotnet user-secrets.");
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

if (allowedOrigins.Length == 0)
{
    throw new InvalidOperationException(
        "Falta la configuración Cors:AllowedOrigins para el ambiente actual.");
}

builder.Services.AddOpenApi();
builder.Services.AddApiErrorHandling();
builder.Services.AddAirportCaching();
builder.Services.AddAuthModule(builder.Configuration, connectionString);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AirportWeb", policy =>
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.Services.AddFlightsModule(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseApiErrorHandling();
app.UseHttpsRedirection();
app.UseCors("AirportWeb");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
    service = "Airport.Api",
    status = "ready"
}));

app.MapFlightsModule();
app.MapAuthModule();

app.Run();
