using Airport.Features.Flights.Domain;

namespace Airport.Features.Flights.Application.Ports;

public interface IFlightReader
{
    Task<Flight?> FindByIdAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Airport.Features.Flights.Domain.Airport>> ListAirportsAsync(
        int? originAirportId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AirlineFilterOption>> ListAirlinesAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AirplaneFilterOption>> ListAirplanesAsync(
        short airlineId,
        CancellationToken cancellationToken);

    Task<FlightSearchPage> SearchAsync(
        FlightSearchCriteria criteria,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}
