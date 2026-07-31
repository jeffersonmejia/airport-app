using Airport.Features.Flights.Application.SearchFlights;

namespace Airport.UnitTests.Flights;

public sealed class SearchFlightsValidatorTests
{
    private readonly SearchFlightsValidator validator = new();

    [Fact]
    public void Validate_WithValidPagination_ReturnsNoErrors()
    {
        var errors = validator.Validate(new SearchFlightsQuery("AE", 1, 5));

        Assert.Empty(errors);
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(1, 0)]
    [InlineData(1, 6)]
    public void Validate_WithInvalidPagination_ReturnsErrors(int page, int pageSize)
    {
        var errors = validator.Validate(new SearchFlightsQuery(null, page, pageSize));

        Assert.NotEmpty(errors);
    }
}
