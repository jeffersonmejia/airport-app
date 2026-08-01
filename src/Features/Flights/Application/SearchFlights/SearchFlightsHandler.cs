using Airport.Features.Flights.Application.Ports;

namespace Airport.Features.Flights.Application.SearchFlights;

public sealed class SearchFlightsHandler(IFlightReader flightReader)
{
    public async Task<SearchFlightsResponse> HandleAsync(
        SearchFlightsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await flightReader.SearchAsync(
            new FlightSearchCriteria(
                query.OriginAirportId,
                query.DestinationAirportId,
                query.DepartureDate,
                query.Number?.Trim(),
                query.SortBy,
                query.Descending,
                query.AirlineId,
                query.AirplaneId),
            query.Page,
            query.PageSize,
            cancellationToken);
        var items = result.Items.Select(SearchFlightItemResponse.FromDomain).ToArray();
        var totalPages = result.TotalItems == 0
            ? 0
            : (int)Math.Ceiling(result.TotalItems / (double)query.PageSize);

        return new SearchFlightsResponse(
            items,
            query.Page,
            query.PageSize,
            result.TotalItems,
            totalPages);
    }
}
