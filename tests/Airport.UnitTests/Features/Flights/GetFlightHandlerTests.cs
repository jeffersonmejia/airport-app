using Airport.Features.Flights.Application.GetFlight;
using Airport.Features.Flights.Application.Ports;
using Airport.Features.Flights.Domain;

namespace Airport.UnitTests.Flights;

public sealed class GetFlightHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenFlightExists_ReturnsProjectedResponse()
    {
        var flight = new Flight(
            42,
            "AE042   ",
            new DateTimeOffset(2026, 7, 30, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 7, 30, 12, 30, 0, TimeSpan.Zero),
            7,
            99);
        var handler = new GetFlightHandler(new StubFlightReader(flight));

        var response = await handler.HandleAsync(new GetFlightQuery(42), CancellationToken.None);

        Assert.NotNull(response);
        Assert.Equal("AE042", response.Number);
        Assert.Equal(2.5, response.DurationHours);
    }

    [Fact]
    public async Task HandleAsync_WhenFlightDoesNotExist_ReturnsNull()
    {
        var handler = new GetFlightHandler(new StubFlightReader(null));

        var response = await handler.HandleAsync(new GetFlightQuery(404), CancellationToken.None);

        Assert.Null(response);
    }

    private sealed class StubFlightReader(Flight? flight) : IFlightReader
    {
        public Task<Flight?> FindByIdAsync(int id, CancellationToken cancellationToken) =>
            Task.FromResult(flight);

        public Task<IReadOnlyList<Airport.Features.Flights.Domain.Airport>> ListAirportsAsync(
            int? originAirportId,
            CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Airport.Features.Flights.Domain.Airport>>([]);

        public Task<FlightSearchPage> SearchAsync(
            FlightSearchCriteria criteria,
            int page,
            int pageSize,
            CancellationToken cancellationToken) =>
            Task.FromResult(new FlightSearchPage([], 0));
    }
}
