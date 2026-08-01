using Airport.Features.Flights.Application.SearchFlights;

namespace Airport.UnitTests.Flights;

public sealed class SearchFlightsValidatorTests
{
    private readonly SearchFlightsValidator validator = new();

    [Fact]
    public void Validate_WithValidPagination_ReturnsNoErrors()
    {
        var errors = validator.Validate(new SearchFlightsQuery(null, null, null, "AE", "departure", false, 1, 5));

        Assert.Empty(errors);
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(1, 0)]
    [InlineData(1, 6)]
    public void Validate_WithInvalidPagination_ReturnsErrors(int page, int pageSize)
    {
        var errors = validator.Validate(new SearchFlightsQuery(null, null, null, null, "departure", false, page, pageSize));

        Assert.NotEmpty(errors);
    }

    [Theory]
    [InlineData(0, null)]
    [InlineData(null, 0)]
    public void Validate_WithInvalidTransportIds_ReturnsErrors(short? airlineId, int? airplaneId)
    {
        var query = new SearchFlightsQuery(
            null, null, null, null, "departure", false, 1, 5,
            airlineId, airplaneId);

        var errors = validator.Validate(query);

        Assert.NotEmpty(errors);
    }
}
