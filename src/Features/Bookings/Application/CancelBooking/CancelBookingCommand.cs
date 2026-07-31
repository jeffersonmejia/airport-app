namespace Airport.Features.Bookings.Application.CancelBooking;

public sealed record CancelBookingCommand(int BookingId, int EmployeeId, string Reason);
