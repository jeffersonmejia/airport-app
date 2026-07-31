# 007 — Fix: paginación dinámica del CRUD de reservas

## Objetivo

Hacer que la paginación de la tabla CRUD de `Bookings` sea dinámica: la UI debe
indicar el total de registros y el número exacto de páginas disponibles, de forma
similar al listado de `Flights`. La solución debe ser mínima, confinada a la feature
`Bookings` y sin perjudicar a otros módulos.

## Verificación del problema

Inspección directa del código actual:

| Ubicación | Situación |
|---|---|
| `Bookings.razor:116-120` | La barra de paginación muestra sólo `Página [X] · 5 por página` con botones `Anterior`/`Siguiente`. No informa el total de páginas ni el total de registros. |
| `SearchBookingsResponse.cs` | La respuesta sólo expone `Items`, `Page`, `PageSize` y `HasNextPage`. No hay `TotalItems` ni `TotalPages`. |
| `BookingSearchViewModel.cs` | El view model web replica la misma carencia (`HasNextPage`, sin totales). |
| `PostgresBookingRepository.SearchAsync:25-36` | Detecta `HasNextPage` consultando `pageSize + 1` filas; no hay conteo. |
| `Flights.razor:123-137` y `SearchFlightsResponse.cs` | Referencia funcional: `Flights` ya implementa `TotalItems` + `TotalPages` y muestra `Página X de Y` y `N registros`. |

La característica más pequeña que permite mostrar "páginas totales" y "registros
totales" es un conteo del total. El reto es que `airportdb.booking` tiene ~54M de
registros (`006_account.md:18-21`), y el plan original evitó explícitamente ejecutar
`COUNT(*)` global en cada solicitud (`006_account.md:110-114`).

## Estrategia elegida: conteo híbrido

- **Con filtros** (`booking_id`, `flight_id` o `passenger_id`): `CountAsync()` exacto
  sobre la consulta filtrada. Todos los filtros usan índices (`006_account.md:27`),
  por lo que el conteo es acotado y barato.
- **Sin filtros** (vista general): total aproximado desde `pg_class.reltuples`, el
  mismo catálogo que ya usa el panel administrativo
  (`PostgresDatabaseSummaryReader.EstimatesSql`). No recorre las 54M filas y cumple
  la premisa de `006_account.md:112` de presentar el total general como aproximado.

La UI mostrará `≈ 54.311.152 registros` (con la marca "aproximado") en la vista
general y un número exacto cuando existan filtros activos.

## Cambios mínimos por capa

Todos los cambios están dentro de la feature `Bookings`. Ninguna otra feature se
modifica.

### Aplicación

| Archivo | Cambio |
|---|---|
| `Application/Ports/BookingPage.cs` | Añadir `int TotalItems` y `bool TotalApproximate` al record. |
| `Application/SearchBookings/SearchBookingsResponse.cs` | Añadir `int TotalItems`, `int TotalPages` y `bool TotalApproximate`. |
| `Application/SearchBookings/SearchBookingsHandler.cs` | Calcular `TotalPages` (`ceil(TotalItems / PageSize)`, con `0` cuando no hay registros) y propagar los totales. |

### Infraestructura

| Archivo | Cambio |
|---|---|
| `Infrastructure/Persistence/PostgresBookingRepository.cs` | En `SearchAsync`, calcular `TotalItems` con la estrategia híbrida descrita: `CountAsync` con filtros y `reltuples` sin filtros. Se conserva el truco de `pageSize + 1` para `HasNextPage`. |

### Presentación Web

| Archivo | Cambio |
|---|---|
| `Presentation/Web/Models/BookingSearchViewModel.cs` | Añadir `TotalItems`, `TotalPages` y `TotalApproximate`. |
| `Presentation/Web/Pages/Bookings.razor` | Reemplazar el texto de la barra por `Página [X] de [Y]` y añadir `N registros` (con `≈` y "aproximado" cuando corresponda). |

No cambian: contrato de autenticación, políticas, cache (`CachedBookingRepository` se
beneficia automáticamente porque el total viaja dentro de `BookingPage`), endpoints de
mutación, ni el módulo `Flights`.

## Caché y rendimiento

- El total queda dentro de `BookingPage`, por lo que `CachedBookingRepository`
  (clave `bookings:{version}:search:...`) lo conserva sin cambios adicionales.
- `reltuples` es una lectura de catálogo en memoria (sub-milisegundo); no toca la
  tabla.
- `CountAsync` con filtros se resuelve con los índices existentes de `booking_id`,
  `flight_id` y `passenger_id`.
- No se añaden consultas en los flujos de detalle ni de mutación.

## Criterios de aceptación

- La barra muestra el total de registros y `Página [X] de [Y]`.
- La vista general muestra el total con la marca de aproximado (`≈`).
- Con filtros activos el total es exacto.
- Ningún otro módulo cambia y el build completo pasa.
- `HasNextPage` y los botones `Anterior`/`Siguiente` siguen funcionando igual.

## Checklist de implementación

- [x] Redactar este plan.
- [x] Añadir `TotalItems`/`TotalApproximate` a `BookingPage`.
- [x] Implementar conteo híbrido en `PostgresBookingRepository.SearchAsync`.
- [x] Propagar `TotalItems`/`TotalPages` en `SearchBookingsResponse` y el handler.
- [x] Actualizar `BookingSearchViewModel`.
- [x] Actualizar la paginación en `Bookings.razor`.
- [x] Compilar y ejecutar las pruebas unitarias.
