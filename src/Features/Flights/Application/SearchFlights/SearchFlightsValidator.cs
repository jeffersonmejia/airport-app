namespace Airport.Features.Flights.Application.SearchFlights;

public sealed class SearchFlightsValidator
{
    public IReadOnlyDictionary<string, string[]> Validate(SearchFlightsQuery query)
    {
        var errors = new Dictionary<string, string[]>();

        if (query.Page < 1)
        {
            errors[nameof(query.Page)] = ["La página debe ser mayor que cero."];
        }

        if (query.PageSize is < 1 or > 50)
        {
            errors[nameof(query.PageSize)] = ["El tamaño de página debe estar entre 1 y 50."];
        }

        if (query.Number?.Length > 8)
        {
            errors[nameof(query.Number)] = ["El número de vuelo admite máximo 8 caracteres."];
        }

        return errors;
    }
}
