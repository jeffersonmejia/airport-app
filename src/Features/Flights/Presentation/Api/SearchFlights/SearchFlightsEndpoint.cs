using Airport.Features.Flights.Application.SearchFlights;
using Airport.SharedKernel.Pagination;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Airport.Features.Flights.Presentation.Api.SearchFlights;

public static class SearchFlightsEndpoint
{
    public static RouteGroupBuilder MapSearchFlights(this RouteGroupBuilder group)
    {
        group.MapGet("/", HandleAsync)
            .WithName("SearchFlights")
            .WithSummary("Busca vuelos con paginación")
            .Produces<SearchFlightsResponse>()
            .ProducesValidationProblem();

        return group;
    }

    private static async Task<IResult> HandleAsync(
        string? number,
        int? originAirportId,
        int? destinationAirportId,
        DateOnly? departureDate,
        short? airlineId,
        int? airplaneId,
        string? sortBy,
        bool? descending,
        int? page,
        int? pageSize,
        SearchFlightsValidator validator,
        SearchFlightsHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new SearchFlightsQuery(
            originAirportId,
            destinationAirportId,
            departureDate,
            number,
            sortBy?.Trim().ToLowerInvariant() ?? "departure",
            descending ?? false,
            page ?? PaginationPolicy.DefaultPage,
            pageSize ?? PaginationPolicy.PageSize,
            airlineId,
            airplaneId);
        var errors = validator.Validate(query);

        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        var response = await handler.HandleAsync(query, cancellationToken);
        return Results.Ok(response);
    }
}
