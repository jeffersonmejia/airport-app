namespace Airport.Features.Auth.Application.Ports;

public interface IActiveSessionRegistry
{
    ValueTask ActivateAsync(
        int userId,
        string sessionId,
        TimeSpan lifetime,
        CancellationToken cancellationToken);

    ValueTask<bool> IsActiveAsync(
        int userId,
        string sessionId,
        CancellationToken cancellationToken);

    ValueTask RevokeAsync(
        int userId,
        string sessionId,
        CancellationToken cancellationToken);
}
