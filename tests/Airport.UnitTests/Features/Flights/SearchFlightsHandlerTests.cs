using Airport.Features.Flights.Application.Ports;
using Airport.Features.Flights.Application.SearchFlights;
using Airport.Features.Flights.Domain;

namespace Airport.UnitTests.Flights;

public sealed class SearchFlightsHandlerTests
{
    [Fact]
    public async Task HandleAsync_MapsItemsAndCalculatesTotalPages()
    {
        var flights = new[]
        {
            new Flight(
                7,
                "AE007   ",
                new DateTimeOffset(2026, 7, 30, 8, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 7, 30, 9, 30, 0, TimeSpan.Zero),
                2,
                4)
        };
        var handler = new SearchFlightsHandler(new StubFlightReader(flights, 17));

        var result = await handler.HandleAsync(
            new SearchFlightsQuery(null, null, null, null, "departure", false, 2, 5),
            CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal("AE007", result.Items[0].Number);
        Assert.Equal(4, result.TotalPages);
        Assert.Equal(17, result.TotalItems);
    }

    private sealed class StubFlightReader(IReadOnlyList<Flight> flights, int totalItems)
        : IFlightReader
    {
        public Task<Flight?> FindByIdAsync(int id, CancellationToken cancellationToken) =>
            Task.FromResult<Flight?>(null);

        public Task<IReadOnlyList<Airport.Features.Flights.Domain.Airport>> ListAirportsAsync(
            CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Airport.Features.Flights.Domain.Airport>>([]);

        public Task<FlightSearchPage> SearchAsync(
            FlightSearchCriteria criteria,
            int page,
            int pageSize,
            CancellationToken cancellationToken) =>
            Task.FromResult(new FlightSearchPage(flights, totalItems));
    }
}
