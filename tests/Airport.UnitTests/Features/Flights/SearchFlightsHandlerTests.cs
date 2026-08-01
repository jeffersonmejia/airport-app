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
                2, 4),
            CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal("AE007", result.Items[0].Number);
        Assert.Equal(4, result.TotalPages);
        Assert.Equal(17, result.TotalItems);
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
            int? originAirportId,
            CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Airport.Features.Flights.Domain.Airport>>([]);

        public Task<IReadOnlyList<DateOnly>> ListDepartureDatesAsync(
            int originAirportId,
            int destinationAirportId,
            CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<DateOnly>>([]);

        public Task<IReadOnlyList<AirlineFilterOption>> ListAirlinesAsync(
            FlightRouteFilter route,
            CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<AirlineFilterOption>>([]);

        public Task<IReadOnlyList<AirplaneFilterOption>> ListAirplanesAsync(
            short airlineId,
            FlightRouteFilter route,
            CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<AirplaneFilterOption>>([]);

        public Task<IReadOnlyList<string>> ListFlightNumbersAsync(
            FlightNumberFilter filter,
            CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<string>>([]);

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
