using Airport.Features.Flights.Application.Ports;

namespace Airport.Features.Flights.Application.ListAirports;

public sealed class ListAirportsHandler(IFlightReader flightReader)
{
    public async Task<IReadOnlyCollection<AirportResponse>> HandleAsync(
        CancellationToken cancellationToken) =>
        (await flightReader.ListAirportsAsync(cancellationToken))
            .Select(airport => new AirportResponse(
                airport.Id,
                airport.Iata,
                airport.Icao,
                airport.Name))
            .ToArray();
}

public sealed record AirportResponse(int Id, string Iata, string Icao, string Name);
