namespace Airport.Features.Bookings.Application.CreateOrder;

public sealed record CreateOrderCommand(
    string UserId,
    int FlightId,
    string FareCode,
    string PassengerFirstName,
    string PassengerLastName,
    string PassportNumber);
