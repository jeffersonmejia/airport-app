using Airport.Core.Flights.Domain;

namespace Airport.Core.Flights.Ports;

public interface IFlightReader
{
    Task<Flight?> FindByIdAsync(int id, CancellationToken cancellationToken);
}
