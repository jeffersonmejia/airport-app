using Airport.Core.Flights.Domain;
using Airport.Core.Flights.Features.GetFlight;
using Airport.Core.Flights.Ports;

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
    }
}
