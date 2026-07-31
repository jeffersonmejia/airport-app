using Airport.Features.Flights.Domain;

namespace Airport.Features.Flights.Application.Ports;

public interface IFlightReader
{
    Task<Flight?> FindByIdAsync(int id, CancellationToken cancellationToken);

    Task<FlightSearchPage> SearchAsync(
        string? number,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}
