using Airport.Features.Flights.Domain;

namespace Airport.Features.Flights.Application.Ports;

public interface IFlightReader
{
    Task<Flight?> FindByIdAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Airport.Features.Flights.Domain.Airport>> ListAirportsAsync(
        int? originAirportId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<DateOnly>> ListDepartureDatesAsync(
        int originAirportId,
        int destinationAirportId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AirlineFilterOption>> ListAirlinesAsync(
        FlightRouteFilter route,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AirplaneFilterOption>> ListAirplanesAsync(
        short airlineId,
        FlightRouteFilter route,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<string>> ListFlightNumbersAsync(
        FlightNumberFilter filter,
        CancellationToken cancellationToken);

    Task<FlightSearchPage> SearchAsync(
        FlightSearchCriteria criteria,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}
