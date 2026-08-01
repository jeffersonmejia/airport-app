using Airport.Features.Bookings.Application.Ports;
using Airport.Features.Bookings.Domain;

namespace Airport.Features.Bookings.Application.CreateOrder;

public sealed class CreateOrderHandler(IBookingRepository repository, TimeProvider timeProvider)
{
    public async Task<CreateOrderResponse?> HandleAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var flight = await repository.FindFlightOfferAsync(command.FlightId, cancellationToken);
        if (flight is null) return null;

        var fare = TicketFare.FromFlight(command.FareCode, flight.Departure, flight.Arrival);
        if (fare is null) return null;

        var order = new TicketOrder(
            Guid.NewGuid(),
            command.UserId,
            flight.FlightId,
            flight.FlightNumber.Trim(),
            flight.OriginCode,
            flight.DestinationCode,
            flight.Departure,
            fare.Code,
            fare.Name,
            fare.Price,
            "USD",
            TicketOrder.PendingPayment,
            command.PassengerFirstName.Trim(),
            command.PassengerLastName.Trim(),
            command.PassportNumber.Trim().ToUpperInvariant(),
            timeProvider.GetUtcNow());
        await repository.AddAsync(order, cancellationToken);
        return CreateOrderResponse.FromDomain(order);
    }
}

public sealed record CreateOrderResponse(
    Guid OrderId,
    string Status,
    decimal Total,
    string CurrencyCode,
    string FlightNumber,
    string Route,
    DateTimeOffset Departure,
    string FareName)
{
    public static CreateOrderResponse FromDomain(TicketOrder order) => new(
        order.Id,
        order.Status,
        order.Total,
        order.CurrencyCode,
        order.FlightNumber,
        $"{order.OriginCode} → {order.DestinationCode}",
        order.Departure,
        order.FareName);
}
