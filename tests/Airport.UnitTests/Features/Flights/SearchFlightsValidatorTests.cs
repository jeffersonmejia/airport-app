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
    [InlineData("UI", null)]
    [InlineData("UIO1", null)]
    [InlineData(null, "TOOLONG")]
    public void Validate_WithInvalidAirportCode_ReturnsErrors(string? originCode, string? destinationCode)
    {
        var query = new SearchFlightsQuery(
            null, null, null, null, "departure", false, 1, 5,
            originCode, destinationCode);

        var errors = validator.Validate(query);

        Assert.NotEmpty(errors);
    }

    [Fact]
    public void Validate_WithSameRouteCode_ReturnsDestinationError()
    {
        var query = new SearchFlightsQuery(
            null, null, null, null, "departure", false, 1, 5,
            "uio", "UIO");

        var errors = validator.Validate(query);

        Assert.Contains(nameof(SearchFlightsQuery.DestinationCode), errors.Keys);
    }
}
