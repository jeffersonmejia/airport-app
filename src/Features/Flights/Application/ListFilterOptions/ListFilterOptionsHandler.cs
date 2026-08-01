using Airport.Features.Flights.Application.Ports;

namespace Airport.Features.Flights.Application.ListFilterOptions;

public sealed class ListFilterOptionsHandler(IFlightReader flightReader)
{
    public Task<IReadOnlyList<DateOnly>> ListDepartureDatesAsync(
        int originAirportId,
        int destinationAirportId,
        CancellationToken cancellationToken) =>
        flightReader.ListDepartureDatesAsync(
            originAirportId,
            destinationAirportId,
            cancellationToken);

    public async Task<IReadOnlyCollection<AirlineOptionResponse>> ListAirlinesAsync(
        FlightRouteFilter route,
        CancellationToken cancellationToken) =>
        (await flightReader.ListAirlinesAsync(route, cancellationToken))
            .Select(option => new AirlineOptionResponse(option.Id, option.Iata, option.Name))
            .ToArray();

    public async Task<IReadOnlyCollection<AirplaneOptionResponse>> ListAirplanesAsync(
        short airlineId,
        FlightRouteFilter route,
        CancellationToken cancellationToken) =>
        (await flightReader.ListAirplanesAsync(airlineId, route, cancellationToken))
            .Select(option => new AirplaneOptionResponse(option.Id, option.Model, option.Capacity))
            .ToArray();

    public Task<IReadOnlyList<string>> ListFlightNumbersAsync(
        int originAirportId,
        int destinationAirportId,
        DateOnly? departureDate,
        short? airlineId,
        int? airplaneId,
        CancellationToken cancellationToken) =>
        flightReader.ListFlightNumbersAsync(
            new FlightNumberFilter(
                originAirportId,
                destinationAirportId,
                departureDate,
                airlineId,
                airplaneId),
            cancellationToken);
}

public sealed record AirlineOptionResponse(short Id, string Iata, string Name);

public sealed record AirplaneOptionResponse(int Id, string Model, int Capacity);
