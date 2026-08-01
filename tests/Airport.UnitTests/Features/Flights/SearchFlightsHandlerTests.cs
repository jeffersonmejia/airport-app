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
        var reader = new StubFlightReader(flights, 17);
        var handler = new SearchFlightsHandler(reader);

        var result = await handler.HandleAsync(
            new SearchFlightsQuery(
                null, null, null, null, "departure", false, 2, 5,
                "uio", " gye ", 2, 4),
            CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal("AE007", result.Items[0].Number);
        Assert.Equal(4, result.TotalPages);
        Assert.Equal(17, result.TotalItems);
        Assert.Equal("UIO", reader.LastCriteria?.OriginCode);
        Assert.Equal("GYE", reader.LastCriteria?.DestinationCode);
        Assert.Equal((short)2, reader.LastCriteria?.AirlineId);
        Assert.Equal(4, reader.LastCriteria?.AirplaneId);
    }

    private sealed class StubFlightReader(IReadOnlyList<Flight> flights, int totalItems)
        : IFlightReader
    {
        public FlightSearchCriteria? LastCriteria { get; private set; }

        public Task<Flight?> FindByIdAsync(int id, CancellationToken cancellationToken) =>
            Task.FromResult<Flight?>(null);

        public Task<IReadOnlyList<Airport.Features.Flights.Domain.Airport>> ListAirportsAsync(
            CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Airport.Features.Flights.Domain.Airport>>([]);

        public Task<FlightSearchPage> SearchAsync(
            FlightSearchCriteria criteria,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            LastCriteria = criteria;
            return Task.FromResult(new FlightSearchPage(flights, totalItems));
        }
    }
}
