# 006 — Accounting y gestión de reservas

## Objetivo

Implementar la gestión de `airportdb.booking` como una feature vertical `Bookings`,
protegida principalmente por el rol `Accounting`. La feature permitirá consultar,
crear, corregir y cancelar reservas con reglas compatibles con el esquema real, sin
exponer nombres físicos de tablas ni ejecutar operaciones masivas sobre los más de
54 millones de registros existentes.

`Bookings` será el nombre de la feature porque representa el concepto del negocio;
`Accounting` será una política de acceso, no una capa ni un módulo técnico.

## Evidencia del esquema actual

La decisión se basa en una inspección directa de PostgreSQL:

| Dato | Resultado |
|---|---|
| Filas estimadas | 54.311.152 |
| Tamaño total aproximado | 6.658 MB |
| Clave primaria | `booking_id` |
| Relación de vuelo | `flight_id` → `flight.flight_id` |
| Relación de pasajero | `passenger_id` → `passenger.passenger_id` |
| Asiento | `seat`, `character(4)`, admite `NULL` |
| Precio | `price`, `numeric(10,2)`, obligatorio |
| Asiento único | Índice único `(flight_id, seat)` |
| Índices de búsqueda | `flight_id`, `passenger_id` y `booking_id` |
| Secuencia para identificadores | No existe para `booking_id` |
| Estado de reserva | No existe |
| Fecha o motivo de cancelación | No existen |
| Triggers de auditoría | No existen |
| Fechas de vuelos | Del 01/06/2015 al 01/09/2015; no existen vuelos futuros |

Las claves foráneas no declaran borrado en cascada. PostgreSQL impedirá eliminar un
vuelo o pasajero que todavía tenga reservas relacionadas, pero actualmente no evita
el borrado físico de una reserva ni conserva quién lo ejecutó.

## Autorización

Se definirán políticas declarativas, sin comprobar roles manualmente dentro de los
handlers:

| Acción | `Accounting` | `Admin` | Otros roles |
|---|---:|---:|---:|
| Listar y ver detalle | Sí | Sí | No |
| Crear una reserva | Sí | Sí | No |
| Corregir asiento o precio | Sí | Sí | No |
| Cancelar una reserva futura | Sí | Sí | No |
| Eliminar físicamente | No | No | No |

`Marketing` podrá recibir posteriormente estadísticas agregadas y anónimas, pero no
acceso a reservas individuales ni datos del pasajero. `Logistics` y
`AirfieldOperations` no administrarán información económica de `booking`.

Políticas previstas:

- `BookingsRead`: `Accounting` o `Admin`.
- `BookingsWrite`: `Accounting` o `Admin`.
- `BookingsCancel`: `Accounting` o `Admin`.

Aunque inicialmente compartan los mismos roles, se mantendrán como políticas
separadas para poder restringir acciones sin modificar endpoints ni handlers.

## Decisión sobre cancelación

No se expondrá `DELETE /api/bookings/{id}`. En el esquema actual, borrar sería la
única forma aparente de “cancelar”, pero destruiría un registro financiero e impediría
saber quién realizó la acción y por qué.

Antes de habilitar cancelaciones se agregará una tabla pequeña, propiedad de la
aplicación, sin reescribir los 54 millones de registros:

```text
booking_cancellation
├── booking_id      PK y FK → booking.booking_id
├── cancelled_at    timestamptz, obligatorio
├── cancelled_by    FK → employee.employee_id
└── reason          varchar(250), obligatorio
```

Una reserva estará activa cuando no tenga un registro en `booking_cancellation`. La
operación `CancelBooking` insertará dicho registro dentro de una transacción. No se
permitirá cancelar dos veces ni cancelar un vuelo cuya salida ya ocurrió.

Esta ampliación es una migración de esquema controlada, no un seeder. Se guardará en
un script pequeño, idempotente y separado del dump original. Antes de ejecutarlo se
realizará respaldo y se validará en una copia local.

## Identificadores para nuevas reservas

`booking_id` es obligatorio, pero no tiene `IDENTITY`, valor por defecto ni secuencia.
Calcular `MAX(booking_id) + 1` desde la aplicación sería lento y produciría colisiones
con solicitudes concurrentes.

Se creará `airportdb.booking_booking_id_seq`, se sincronizará una sola vez con el
máximo existente y se asignará como valor por defecto de `booking.booking_id`. La
secuencia quedará asociada a la columna. Esta preparación también será una migración,
no un seeder.

## Casos de uso

### `SearchBookings`

- Devuelve como máximo 5 elementos, sin opción de aumentar el tamaño.
- Acepta filtros por `booking_id`, `flight_id` o `passenger_id` para aprovechar los
  índices existentes.
- Ordena de forma determinista por `booking_id` descendente.
- Limita `page` al rango definido por `PaginationPolicy` (`1..10.000`).
- Para saber si existe una página siguiente consulta 6 filas y devuelve sólo 5; no
  ejecuta `COUNT(*)` sobre toda la tabla en cada solicitud.
- El total general se presenta como aproximado usando el resumen administrativo.
- Una búsqueda filtrada podrá calcular un total exacto únicamente cuando el filtro
  use un índice y el costo sea acotado.

### `GetBooking`

- Busca exclusivamente por la clave primaria.
- Devuelve identificadores y datos funcionales mínimos de vuelo, pasajero, asiento,
  precio y estado de cancelación.
- No devuelve credenciales ni información personal que la pantalla no necesite.

### `CreateBooking`

- Requiere pasajero y vuelo existentes.
- Sólo permite vuelos cuya salida sea posterior a la hora actual.
- Exige precio entre `0,01` y `99.999.999,99`.
- Normaliza el asiento a mayúsculas, elimina espacios laterales y respeta los cuatro
  caracteres disponibles.
- Rechaza un asiento ya ocupado mediante el índice único `(flight_id, seat)` y
  transforma la violación en `409 Conflict`.
- Obtiene `booking_id` desde la secuencia de PostgreSQL.

### `UpdateBooking`

- Permite corregir únicamente `seat` y `price` mientras el vuelo sea futuro y la
  reserva continúe activa.
- No permite cambiar `passenger_id` ni `flight_id`; eso convertiría una reserva en
  otra. Se deberá cancelar y crear una nueva.
- Utiliza concurrencia optimista. Si el registro cambió desde que se leyó, devuelve
  `409 Conflict` en vez de sobrescribir silenciosamente.

### `CancelBooking`

- Se expone como `POST /api/bookings/{id}/cancel`, con motivo obligatorio.
- Sólo funciona sobre reservas activas de vuelos futuros.
- Registra el identificador del empleado autenticado, la hora UTC y el motivo.
- Una segunda cancelación devuelve `409 Conflict` de forma idempotente y comprensible.
- No elimina la fila de `booking`.

## Alcance de los datos históricos

Todos los vuelos actuales terminaron en 2015. Por tanto, sobre el dump restaurado:

- `SearchBookings` y `GetBooking` funcionarán normalmente.
- Las reservas históricas no podrán editarse ni cancelarse.
- `CreateBooking` sólo será posible cuando exista un vuelo futuro válido.
- Los intentos de mutar datos históricos devolverán `409 Conflict` con un mensaje
  seguro.

Esta restricción es intencional: el rol `Accounting` puede consultar los registros
históricos, pero no reescribir hechos financieros pasados. Para demostrar las
mutaciones se necesitará primero un flujo autorizado que cree vuelos futuros o una
prueba de integración transaccional que se revierta al finalizar.

## API prevista

| Método y ruta | Slice | Resultado principal |
|---|---|---|
| `GET /api/bookings` | `SearchBookings` | Página de hasta 5 reservas |
| `GET /api/bookings/{id}` | `GetBooking` | Detalle de una reserva |
| `POST /api/bookings` | `CreateBooking` | `201 Created` |
| `PATCH /api/bookings/{id}` | `UpdateBooking` | Reserva corregida |
| `POST /api/bookings/{id}/cancel` | `CancelBooking` | Cancelación registrada |

Respuestas esperadas:

- `400`: formato o paginación inválidos.
- `401`: no existe una sesión válida.
- `403`: el rol no tiene acceso.
- `404`: reserva, vuelo o pasajero inexistente.
- `409`: asiento ocupado, reserva cancelada, dato histórico o conflicto concurrente.

## Persistencia, rendimiento y caché

- Todo acceso a PostgreSQL se implementará con EF Core y Npgsql dentro de
  `Bookings/Infrastructure`.
- Las lecturas utilizarán `AsNoTracking`, proyección y cancelación.
- No se materializarán entidades de pasajero o vuelo completas para formar el DTO.
- No habrá listado sin `Take(5)` ni consultas exactas globales por solicitud.
- Las lecturas se conservarán hasta 30 segundos en `IApplicationCache`, respetando el
  máximo global de 256 entradas.
- Crear, actualizar o cancelar invalidará las claves de caché afectadas.
- Las escrituras utilizarán transacciones breves; no se mantendrán bloqueos mientras
  se espera interacción del usuario.
- Se revisará el plan de ejecución con `EXPLAIN (ANALYZE, BUFFERS)` sobre consultas
  representativas, sin ejecutar análisis destructivos.

## Interfaz web

El Home de `Accounting` mostrará un acceso “Reservas”. La página incluirá:

- Tabla o lista responsive con exactamente 5 reservas por página.
- Búsqueda por identificador de reserva, vuelo o pasajero.
- Asiento, precio, vuelo, pasajero y estado visible con etiquetas funcionales.
- Detalle accesible sin cargar información personal innecesaria.
- Formularios separados para crear y corregir.
- Diálogo de cancelación con impacto, motivo obligatorio y confirmación explícita.
- Acciones de edición y cancelación deshabilitadas para vuelos históricos o reservas
  canceladas, explicando el motivo mediante texto y no sólo color.
- Estados de carga, vacío, error, éxito, `401`, `403` y conflicto.

## Estructura objetivo

```text
Features/Bookings/
├── Domain/
│   └── Booking.cs
├── Application/
│   ├── SearchBookings/
│   ├── GetBooking/
│   ├── CreateBooking/
│   ├── UpdateBooking/
│   ├── CancelBooking/
│   └── Ports/
├── Infrastructure/
│   ├── Persistence/
│   └── DependencyInjection.cs
└── Presentation/
    ├── Api/
    └── Web/
```

## Pruebas obligatorias

- Mapeo exacto de columnas, tipos, claves e índice único.
- Paginación fija de 5 y límite de página 10.000.
- Consultas ordenadas y proyectadas.
- Autorización positiva para `Accounting` y `Admin`.
- `403` para los demás roles.
- Creación con vuelo futuro y rechazo de vuelo pasado.
- Rechazo de asiento ocupado.
- Actualización limitada a asiento y precio.
- Cancelación auditada sin borrar `booking`.
- Rechazo de cancelación histórica o duplicada.
- Invalidación de caché después de cada escritura.
- Integración con PostgreSQL dentro de una transacción reversible.

## Implementación por etapas

- [x] Crear los proyectos de la feature `Bookings` sólo al iniciar su implementación.
- [x] Añadir la secuencia segura para `booking_id` mediante script idempotente.
- [x] Añadir `booking_cancellation` mediante script idempotente.
- [x] Mapear `booking` y las relaciones mínimas con EF Core.
- [x] Implementar `SearchBookings` y `GetBooking` con paginación máxima de 5.
- [x] Definir y aplicar las tres políticas de autorización.
- [x] Implementar `CreateBooking` con validaciones e índice único.
- [x] Implementar `UpdateBooking` con campos limitados y concurrencia optimista.
- [x] Implementar `CancelBooking` sin borrado físico.
- [x] Invalidar caché tras mutaciones.
- [x] Construir la página responsive del rol `Accounting`.
- [x] Añadir pruebas unitarias, de arquitectura e integración.
- [x] Verificar rendimiento y accesibilidad.

## Criterios de aceptación

- Ninguna respuesta contiene más de 5 reservas.
- Sólo `Accounting` y `Admin` acceden a registros individuales.
- Los listados no cuentan ni recorren los 54 millones de registros en cada carga.
- Una cancelación conserva la reserva original y registra autor, fecha y motivo.
- No se pueden modificar ni cancelar reservas de vuelos pasados.
- No se genera ningún identificador mediante `MAX + 1`.
- La UI comunica claramente datos aproximados, acciones deshabilitadas y conflictos.
