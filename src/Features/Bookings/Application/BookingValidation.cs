using Airport.SharedKernel.Pagination;

namespace Airport.Features.Bookings.Application;

public static class BookingValidation
{
    public static IReadOnlyDictionary<string, string[]> ValidatePage(int page, int pageSize)
    {
        var errors = new Dictionary<string, string[]>();

        if (page is < PaginationPolicy.DefaultPage or > PaginationPolicy.MaximumPage)
        {
            errors[nameof(page)] = [$"La página debe estar entre 1 y {PaginationPolicy.MaximumPage}." ];
        }

        if (pageSize != PaginationPolicy.PageSize)
        {
            errors[nameof(pageSize)] = [$"Cada página debe contener exactamente {PaginationPolicy.PageSize} elementos."];
        }

        return errors;
    }

    public static IReadOnlyDictionary<string, string[]> ValidateMutation(
        int flightId,
        int passengerId,
        string? seat,
        decimal price)
    {
        var errors = new Dictionary<string, string[]>();
        if (flightId <= 0) errors[nameof(flightId)] = ["El vuelo debe ser válido."];
        if (passengerId <= 0) errors[nameof(passengerId)] = ["El pasajero debe ser válido."];
        if (seat?.Trim().Length > 4) errors[nameof(seat)] = ["El asiento no puede superar 4 caracteres."];
        if (price is < 0.01m or > 99_999_999.99m)
        {
            errors[nameof(price)] = ["El precio debe estar entre 0,01 y 99.999.999,99."];
        }

        return errors;
    }

    public static string? NormalizeSeat(string? seat) =>
        string.IsNullOrWhiteSpace(seat) ? null : seat.Trim().ToUpperInvariant();
}
