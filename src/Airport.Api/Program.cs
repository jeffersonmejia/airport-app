using Airport.Api.Features.Flights.GetFlight;
using Airport.Core.Flights.Features.GetFlight;
using Airport.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AirportDb")
    ?? throw new InvalidOperationException(
        "Falta el secret ConnectionStrings:AirportDb. Configúralo con dotnet user-secrets.");

builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AirportWeb", policy =>
        policy.WithOrigins("http://localhost:5235", "https://localhost:7194")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.Services.AddSingleton<GetFlightValidator>();
builder.Services.AddScoped<GetFlightHandler>();
builder.Services.AddAirportInfrastructure(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AirportWeb");

app.MapGet("/", () => Results.Ok(new
{
    service = "Airport.Api",
    status = "ready"
}));

app.MapGroup("/api/flights")
    .WithTags("Flights")
    .MapGetFlight();

app.Run();
