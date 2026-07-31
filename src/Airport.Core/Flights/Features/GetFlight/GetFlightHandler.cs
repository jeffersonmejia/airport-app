using Airport.Core.Flights.Ports;

namespace Airport.Core.Flights.Features.GetFlight;

public sealed class GetFlightHandler(IFlightReader flightReader)
{
    public async Task<GetFlightResponse?> HandleAsync(
        GetFlightQuery query,
        CancellationToken cancellationToken)
    {
        var flight = await flightReader.FindByIdAsync(query.FlightId, cancellationToken);
        return flight is null ? null : GetFlightResponse.FromDomain(flight);
    }
}
