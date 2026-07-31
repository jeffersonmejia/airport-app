using Airport.SharedKernel.Pagination;

namespace Airport.Features.Flights.Application.SearchFlights;

public sealed class SearchFlightsValidator
{
    public IReadOnlyDictionary<string, string[]> Validate(SearchFlightsQuery query)
    {
        var errors = new Dictionary<string, string[]>();

        if (query.Page is < PaginationPolicy.DefaultPage or > PaginationPolicy.MaximumPage)
        {
            errors[nameof(query.Page)] =
                [$"La página debe estar entre 1 y {PaginationPolicy.MaximumPage}."];
        }

        if (query.PageSize is < 1 or > PaginationPolicy.PageSize)
        {
            errors[nameof(query.PageSize)] =
                [$"El tamaño de página debe estar entre 1 y {PaginationPolicy.PageSize}."];
        }

        if (query.Number?.Length > 8)
        {
            errors[nameof(query.Number)] = ["El número de vuelo admite máximo 8 caracteres."];
        }

        return errors;
    }
}
