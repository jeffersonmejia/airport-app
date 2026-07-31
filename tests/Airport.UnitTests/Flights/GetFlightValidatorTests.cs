using Airport.Core.Flights.Features.GetFlight;

namespace Airport.UnitTests.Flights;

public sealed class GetFlightValidatorTests
{
    private readonly GetFlightValidator _validator = new();

    [Fact]
    public void Validate_WithPositiveId_ReturnsNoError()
    {
        var error = _validator.Validate(new GetFlightQuery(1));

        Assert.Null(error);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithInvalidId_ReturnsError(int id)
    {
        var error = _validator.Validate(new GetFlightQuery(id));

        Assert.NotNull(error);
    }
}
