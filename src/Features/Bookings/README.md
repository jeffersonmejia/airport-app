# Feature Bookings

Bookings gestiona `airportdb.booking` para los roles `Accounting` y `Admin`. Sus
slices son `SearchBookings`, `GetBooking`, `CreateBooking`, `UpdateBooking` y
`CancelBooking`.

Las lecturas respetan páginas de cinco elementos y caché de 30 segundos. Las
escrituras sólo aceptan vuelos futuros; cancelar crea una fila auditada en
`booking_cancellation` y nunca elimina la reserva original.
