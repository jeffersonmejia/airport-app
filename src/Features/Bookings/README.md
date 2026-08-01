# Feature Bookings

Feature del flujo Tipo 1: Compra de boletos, organizada con arquitectura hexagonal
y vertical slices.

Incluye los siguientes casos de uso:

- `CreateOrder`: valida el pasajero, consulta el vuelo real y recalcula la tarifa;
- `GetOrder`: recupera únicamente una orden perteneciente al cliente autenticado;
- `GetHistory`: pagina el historial individual del cliente;
- `GetReceipt`: muestra el comprobante y el boleto emitido.

Infrastructure crea mediante migraciones las tablas propias del esquema
`airport_app`: `orders`, `order_details`, `payments` y `purchased_tickets`. Las tablas
`airportdb.flight` y `airportdb.airport` se consultan como modelo Database First y se
excluyen de esas migraciones.

El CRUD heredado sobre la tabla `booking` no forma parte de este ejercicio y fue
retirado. La integración PayPal implementa el puerto `IPaymentOrderStore` desde esta
feature para completar la compra en una transacción de PostgreSQL.
