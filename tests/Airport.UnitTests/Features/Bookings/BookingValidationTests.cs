using Airport.Features.Bookings.Application;
using Airport.SharedKernel.Pagination;

namespace Airport.UnitTests.Bookings;

public sealed class BookingValidationTests
{
    [Fact]
    public void PageAcceptsExactlyFiveItems()
    {
        var errors = BookingValidation.ValidatePage(1, PaginationPolicy.PageSize);

        Assert.Empty(errors);
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(10001, 5)]
    [InlineData(1, 4)]
    [InlineData(1, 6)]
    public void PageRejectsValuesOutsidePolicy(int page, int pageSize)
    {
        Assert.NotEmpty(BookingValidation.ValidatePage(page, pageSize));
    }

    [Fact]
    public void MutationNormalizesSeatAndValidatesPrice()
    {
        Assert.Equal("12A", BookingValidation.NormalizeSeat(" 12a "));
        Assert.Empty(BookingValidation.ValidateMutation(1, 1, "12a", 125.50m));
        Assert.Contains(
            "price",
            BookingValidation.ValidateMutation(1, 1, "12a", 0m).Keys);
    }
}
