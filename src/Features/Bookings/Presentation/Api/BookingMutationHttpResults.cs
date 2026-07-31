using Airport.Features.Bookings.Application;
using Airport.Features.Bookings.Application.Ports;
using Microsoft.AspNetCore.Http;

namespace Airport.Features.Bookings.Presentation.Api;

internal static class BookingMutationHttpResults
{
    public static IResult ToResult(
        BookingMutationResult result,
        TimeProvider timeProvider,
        bool created = false) => result.Status switch
    {
        BookingMutationStatus.Success when result.Booking is not null && created =>
            Results.Created(
                $"/api/bookings/{result.Booking.BookingId}",
                BookingResponse.FromDomain(result.Booking, timeProvider.GetUtcNow())),
        BookingMutationStatus.Success when result.Booking is not null =>
            Results.Ok(BookingResponse.FromDomain(result.Booking, timeProvider.GetUtcNow())),
        BookingMutationStatus.NotFound => Problem(404, "Reserva no encontrada",
            "La reserva solicitada no existe."),
        BookingMutationStatus.RelatedResourceNotFound => Problem(404, "Relación no encontrada",
            "El vuelo o pasajero seleccionado no existe."),
        BookingMutationStatus.HistoricalFlight => Problem(409, "Vuelo histórico",
            "No se puede modificar una reserva cuyo vuelo ya salió."),
        BookingMutationStatus.AlreadyCancelled => Problem(409, "Reserva cancelada",
            "La reserva ya fue cancelada."),
        BookingMutationStatus.SeatOccupied => Problem(409, "Asiento ocupado",
            "El asiento ya está asignado en ese vuelo."),
        BookingMutationStatus.ConcurrencyConflict => Problem(409, "Conflicto de actualización",
            "La reserva cambió. Actualiza la información e intenta nuevamente."),
        _ => Problem(409, "Operación no completada",
            "No fue posible completar la operación solicitada.")
    };

    private static IResult Problem(int status, string title, string detail) =>
        Results.Problem(title: title, detail: detail, statusCode: status);
}
