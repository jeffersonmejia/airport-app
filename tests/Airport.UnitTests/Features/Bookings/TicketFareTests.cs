using Airport.Features.Bookings.Domain;

namespace Airport.UnitTests.Bookings;

public sealed class TicketFareTests
{
    private static readonly DateTimeOffset Departure =
        new(2026, 8, 2, 10, 0, 0, TimeSpan.Zero);

    [Theory]
    [InlineData("ECONOMY", 121)]
    [InlineData("FLEX", 163.35)]
    [InlineData("BUSINESS", 254.10)]
    public void FromFlightCalculatesServerSideFare(string code, decimal expected)
    {
        var fare = TicketFare.FromFlight(code, Departure, Departure.AddHours(2));

        Assert.NotNull(fare);
        Assert.Equal(expected, fare.Price);
    }

    [Fact]
    public void FromFlightRejectsUnknownFare() =>
        Assert.Null(TicketFare.FromFlight("FREE", Departure, Departure.AddHours(2)));
}
