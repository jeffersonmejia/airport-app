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

        if (query.OriginAirportId is <= 0)
        {
            errors[nameof(query.OriginAirportId)] = ["El aeropuerto de origen no es válido."];
        }

        if (query.DestinationAirportId is <= 0)
        {
            errors[nameof(query.DestinationAirportId)] = ["El aeropuerto de destino no es válido."];
        }

        ValidateAirportCode(query.OriginCode, nameof(query.OriginCode), errors);
        ValidateAirportCode(query.DestinationCode, nameof(query.DestinationCode), errors);

        if (query.OriginAirportId is not null && query.OriginAirportId == query.DestinationAirportId)
        {
            errors[nameof(query.DestinationAirportId)] = ["El destino debe ser diferente del origen."];
        }

        if (!string.IsNullOrWhiteSpace(query.OriginCode) &&
            string.Equals(query.OriginCode.Trim(), query.DestinationCode?.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            errors[nameof(query.DestinationCode)] = ["El destino debe ser diferente del origen."];
        }

        if (query.AirlineId is <= 0)
        {
            errors[nameof(query.AirlineId)] = ["La aerolínea no es válida."];
        }

        if (query.AirplaneId is <= 0)
        {
            errors[nameof(query.AirplaneId)] = ["El avión no es válido."];
        }

        if (query.SortBy is not ("departure" or "arrival" or "number"))
        {
            errors[nameof(query.SortBy)] = ["El ordenamiento solicitado no está permitido."];
        }

        return errors;
    }

    private static void ValidateAirportCode(
        string? code,
        string field,
        IDictionary<string, string[]> errors)
    {
        var normalized = code?.Trim();
        if (!string.IsNullOrEmpty(normalized) &&
            (normalized.Length is < 3 or > 4 || !normalized.All(char.IsLetter)))
        {
            errors[field] = ["El código debe contener 3 o 4 letras (IATA o ICAO)."];
        }
    }
}
