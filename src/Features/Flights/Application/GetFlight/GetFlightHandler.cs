using Airport.Features.Flights.Application.Ports;

namespace Airport.Features.Flights.Application.GetFlight;

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
