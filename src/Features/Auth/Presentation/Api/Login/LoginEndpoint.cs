using Airport.Features.Auth.Application.Login;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Airport.Features.Auth.Presentation.Api.Login;

public static class LoginEndpoint
{
    public static RouteGroupBuilder MapLogin(this RouteGroupBuilder group)
    {
        group.MapPost("/login", HandleAsync)
            .AllowAnonymous()
            .WithName("Login")
            .WithSummary("Inicia una sesión de empleado")
            .Produces<LoginResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return group;
    }

    private static async Task<IResult> HandleAsync(
        LoginRequest request,
        LoginValidator validator,
        LoginHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Username, request.Password);
        var errors = validator.Validate(command);

        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        var response = await handler.HandleAsync(command, cancellationToken);
        return response is null
            ? Results.Problem(
                title: "Credenciales inválidas",
                detail: "El usuario o la contraseña no son correctos.",
                statusCode: StatusCodes.Status401Unauthorized)
            : Results.Ok(response);
    }

    private sealed record LoginRequest(string Username, string Password);
}
