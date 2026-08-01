# Feature Bookings

Frontera reservada para el flujo del Tipo 1: Compra de boletos.

Los casos de uso se incorporarán en las fases correspondientes como vertical slices:

- selección de tarifa;
- creación de orden y detalle;
- procesamiento y verificación del pago;
- registro del boleto adquirido;
- comprobante e historial.

El CRUD heredado sobre la tabla `booking` no forma parte de este ejercicio y fue
retirado. La feature conserva sus proyectos `Domain`, `Application`,
`Infrastructure` y `Presentation` para implementar el flujo correcto sin mezclar
responsabilidades.
