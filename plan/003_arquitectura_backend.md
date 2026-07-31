# 003 — Arquitectura del backend .NET

## Enfoque

Se utilizará .NET 10 con ASP.NET Core y un monolito modular que combine:

- **Screaming architecture:** la estructura principal comunica el dominio
  aeroportuario: Flights, Bookings, Passengers, Airports, Airlines y Weather.
- **Vertical slices:** cada caso de uso agrupa request, validación, handler, response
  y endpoint, en vez de repartirlos en carpetas técnicas globales.
- **Arquitectura hexagonal:** Core define dominio, casos de uso y puertos;
  Infrastructure implementa adaptadores; API recibe HTTP y compone dependencias.

## Features iniciales

| Feature | Tablas principales |
|---|---|
| Airports | `airport`, `airport_geo`, `airport_reachable` |
| Airlines y Fleet | `airline`, `airplane`, `airplane_type` |
| Flights | `flight`, `flight_log`, `flightschedule` |
| Bookings y Passengers | `booking`, `passenger`, `passengerdetails` |
| Operations | `employee`, `weatherdata` |

## Estructura objetivo

```text
Airport.sln
src/
  Airport.Api/                         # HTTP y composition root
  Airport.Core/
    Flights/
      Domain/
      Features/
        GetFlight/
        SearchFlights/
    Bookings/
      Domain/
      Features/
        CreateBooking/
        GetBooking/
    Passengers/
    Airports/
    Airlines/
    Fleet/
    Employees/
    Weather/
    Shared/
      Ports/
  Airport.Infrastructure/              # Adaptadores PostgreSQL y servicios externos
    Persistence/
    Features/
      Flights/
      Bookings/
      Passengers/
      Airports/
  Airport.Web/                         # Cliente web y sistema visual Material
tests/
  Airport.UnitTests/
  Airport.IntegrationTests/
```

## Reglas de dependencia

```text
Airport.Api -------------> Airport.Core
Airport.Infrastructure --> Airport.Core
Airport.Api -------------> Airport.Infrastructure  (sólo composición/DI)
Airport.Web -------------> Airport.Api             (sólo HTTP/contratos)
Airport.Core ------------> ninguna capa externa
```

- Core no referencia ASP.NET Core, EF Core, Npgsql ni componentes visuales.
- Infrastructure conoce los puertos de Core; Core nunca conoce Infrastructure.
- Los endpoints no contienen reglas de negocio.
- Una feature no accede directamente a la persistencia de otra feature.
- Los contratos HTTP no exponen entidades de persistencia.
- Las consultas masivas deben paginarse; la API nunca cargará tablas completas en
  memoria.

## Orden de implementación

- [ ] Crear la solución y proyectos con `dotnet new`.
- [ ] Configurar nullable, warnings y formato compartido.
- [ ] Agregar referencias respetando las reglas hexagonales.
- [ ] Seleccionar versiones de Npgsql y EF Core compatibles con .NET 10.
- [ ] Configurar la conexión mediante user-secrets y un health check.
- [ ] Mapear el esquema existente sin intentar recrear o poblar los 2 GiB mediante
      migraciones.
- [ ] Implementar `Flights/GetFlight` como slice vertical de referencia.
- [ ] Crear pruebas unitarias para Core y de integración para PostgreSQL.
- [ ] Incorporar los demás slices según la prioridad funcional del examen.
- [ ] Añadir paginación, filtros, cancelación y límites de consulta.

Si la aplicación necesita cambios propios de esquema, las migraciones comenzarán
desde una línea base posterior a la importación. El dump académico seguirá siendo la
fuente de la carga inicial.

## Criterio de terminado

- La solución compila con .NET 10.
- Las dependencias respetan el hexágono.
- Existe al menos un slice completo y probado.
- La API conecta con PostgreSQL mediante un rol limitado.
- Las consultas principales están paginadas y no comprometen los 6 GiB de RAM.
