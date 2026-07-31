# 003 — Arquitectura feature-first del sistema .NET

## Objetivo

La solución utilizará .NET 10, ASP.NET Core, Razor/Blazor y Entity Framework Core con
el proveedor Npgsql. EF Core es obligatorio para todo acceso de la aplicación a
PostgreSQL. La organización principal será por características del negocio, no por
capas globales. Al abrir `src/Features` deben verse inmediatamente Flights, Bookings,
Airports y los demás conceptos del dominio: esto aplica screaming architecture.

Cada feature será un módulo vertical autónomo y contendrá:

- **Domain:** entidades, value objects, reglas e invariantes.
- **Application:** casos de uso organizados como vertical slices, además de sus
  puertos de salida.
- **Infrastructure:** adaptadores PostgreSQL e implementaciones de los puertos.
- **Presentation:** endpoints HTTP y componentes/páginas Razor de la feature.

## Tecnologías obligatorias

| Responsabilidad | Tecnología |
|---|---|
| Runtime | .NET 10 |
| Backend HTTP | ASP.NET Core |
| Presentación web | Razor/Blazor |
| ORM | Entity Framework Core 10 |
| Proveedor PostgreSQL | Npgsql Entity Framework Core Provider |
| Base de datos | PostgreSQL 18 nativo |

EF Core se utilizará dentro de `Infrastructure` para:

- Definir el `DbContext` de la aplicación.
- Mapear explícitamente tablas, columnas, claves, relaciones y tipos del esquema
  existente `airportdb` mediante Fluent API.
- Implementar consultas con `AsNoTracking`, proyección, paginación y cancelación.
- Implementar operaciones de escritura y unidades de trabajo cuando el caso de uso
  lo requiera.

No se usará EF Core para restaurar `aeropuerto-db.sql`. Tampoco se crearán migraciones
que intenten volver a insertar los 2 GiB. El dump se restaura con PostgreSQL y EF Core
consume posteriormente ese esquema existente.

## Decisión sobre proyectos

Se usará un proyecto por feature, en lugar de cuatro o cinco proyectos por feature.
Así se conserva la estructura solicitada sin provocar decenas de compilaciones y
restauraciones de paquetes en un equipo de dos núcleos y 6 GB de RAM.

Las fronteras internas se mantendrán mediante:

- Namespaces alineados con las cuatro áreas.
- Tipos `internal` por defecto.
- Un único punto público de registro por módulo.
- Reglas de dependencia documentadas.
- Pruebas de arquitectura posteriores que detecten referencias prohibidas.

Si el profesor exige aislamiento a nivel de ensamblado, cada carpeta podrá convertirse
después en un proyecto independiente sin cambiar la organización funcional.

## Estructura objetivo

```text
Airport.sln
src/
  Hosts/
    Airport.Api/                              # Host HTTP y composition root
    Airport.Web/                              # Host Razor/Blazor y shell visual

  BuildingBlocks/
    Airport.SharedKernel/                     # Abstracciones mínimas compartidas

  Features/
    Flights/
      Airport.Features.Flights.csproj
      Domain/
        Flight.cs
        FlightNumber.cs
      Application/
        Ports/
          IFlightReader.cs
        GetFlight/
          GetFlightQuery.cs
          GetFlightValidator.cs
          GetFlightHandler.cs
          GetFlightResponse.cs
        SearchFlights/
          SearchFlightsQuery.cs
          SearchFlightsValidator.cs
          SearchFlightsHandler.cs
          SearchFlightsResponse.cs
      Infrastructure/
        Persistence/
          AirportDbContext.cs
          FlightRow.cs
          FlightConfiguration.cs
          PostgresFlightReader.cs
      Presentation/
        Api/
          GetFlightEndpoint.cs
          SearchFlightsEndpoint.cs
        Web/
          Pages/
            Flights.razor
          Components/
            FlightCard.razor
            FlightSearch.razor
      FlightsModule.cs                       # Registro público del módulo

    Bookings/
      Airport.Features.Bookings.csproj
      Domain/
      Application/
      Infrastructure/
      Presentation/
        Api/
        Web/
      BookingsModule.cs

    Passengers/
      Airport.Features.Passengers.csproj
      Domain/
      Application/
      Infrastructure/
      Presentation/
        Api/
        Web/
      PassengersModule.cs

    Airports/
      Airport.Features.Airports.csproj
      Domain/
      Application/
      Infrastructure/
      Presentation/
        Api/
        Web/
      AirportsModule.cs

    Airlines/
      Airport.Features.Airlines.csproj
      Domain/
      Application/
      Infrastructure/
      Presentation/
        Api/
        Web/
      AirlinesModule.cs

    Fleet/
      Airport.Features.Fleet.csproj
      Domain/
      Application/
      Infrastructure/
      Presentation/
        Api/
        Web/
      FleetModule.cs

    Employees/
      Airport.Features.Employees.csproj
      Domain/
      Application/
      Infrastructure/
      Presentation/
        Api/
        Web/
      EmployeesModule.cs

    Weather/
      Airport.Features.Weather.csproj
      Domain/
      Application/
      Infrastructure/
      Presentation/
        Api/
        Web/
      WeatherModule.cs

tests/
  Airport.UnitTests/
    Features/
      Flights/
      Bookings/
      Airports/
  Airport.ArchitectureTests/
  Airport.IntegrationTests/
```

## Responsabilidad de los hosts

`Airport.Api` y `Airport.Web` serán hosts delgados. No contendrán reglas de negocio,
repositorios ni casos de uso.

`Airport.Api` se limitará a:

- Cargar configuración y User Secrets.
- Registrar los módulos habilitados.
- Configurar middleware, CORS, errores y OpenAPI.
- Mapear los endpoints expuestos por cada módulo.

`Airport.Web` se limitará a:

- Proporcionar el layout, navegación y tokens visuales globales.
- Registrar los componentes Razor aportados por las features.
- Configurar el cliente HTTP y preocupaciones comunes de interfaz.

Las páginas, formularios y componentes específicos del negocio vivirán en
`Presentation/Web` dentro de su feature.

## Reglas de dependencia internas

```text
Presentation ------> Application ------> Domain
Infrastructure ----> Application ------> Domain
Infrastructure ------------------------> Domain
Domain ------------> ninguna capa del módulo
```

Reglas obligatorias:

1. Domain no referencia Application, Infrastructure, Presentation, EF Core, Npgsql,
   ASP.NET Core ni Razor.
2. Application sólo referencia Domain y contratos mínimos de SharedKernel.
3. Infrastructure implementa los puertos definidos por Application y es la única
   parte de la feature autorizada para usar EF Core, Npgsql y `DbContext`.
4. Presentation invoca handlers de Application; nunca consulta `DbContext`
   directamente.
5. Una feature no accede a las tablas o clases internas de otra feature.
6. La comunicación entre features usa contratos públicos pequeños o eventos, nunca
   referencias a adaptadores internos.
7. Cada feature expone un único archivo `FeatureModule.cs` para registrar servicios,
   endpoints y presentación.
8. Las consultas grandes siempre usan paginación, proyección y cancelación.

## Vertical slice dentro de una feature

Ejemplo para `Flights/Application/GetFlight`:

```text
GetFlightQuery
      |
GetFlightValidator
      |
GetFlightHandler -----> IFlightReader (puerto)
      |                        ^
GetFlightResponse             |
                               |
Infrastructure/PostgresFlightReader
```

`Presentation/Api/GetFlightEndpoint` construye el query y delega en el handler.
`Presentation/Web/Pages/Flights.razor` consume el contrato HTTP y utiliza componentes
de la misma feature. Ninguna de las dos presentaciones contiene reglas de dominio.

## Features y tablas bajo su responsabilidad

| Feature | Tablas |
|---|---|
| Flights | `flight`, `flight_log`, `flightschedule` |
| Bookings | `booking` |
| Passengers | `passenger`, `passengerdetails` |
| Airports | `airport`, `airport_geo`, `airport_reachable` |
| Airlines | `airline` |
| Fleet | `airplane`, `airplane_type` |
| Employees | `employee` |
| Weather | `weatherdata` |

Una relación SQL entre tablas no autoriza a mezclar módulos. Por ejemplo, Flights
puede usar el identificador de Airline, pero los detalles completos de una aerolínea
pertenecen a Airlines.

## Migración de la estructura actual

La implementación existente es un prototipo arquitectónico y debe reorganizarse antes
de añadir más casos de uso.

- [ ] Crear `src/Hosts`, `src/BuildingBlocks` y `src/Features`.
- [ ] Mover `Airport.Api` y `Airport.Web` a `src/Hosts`.
- [ ] Crear el módulo `Airport.Features.Flights`.
- [ ] Mover `Flight` a `Flights/Domain`.
- [ ] Mover query, validator, handler, response y puerto a `Flights/Application`.
- [ ] Mover el mapeo y lector PostgreSQL a `Flights/Infrastructure`.
- [ ] Mantener `AirportDbContext`, configuraciones Fluent API y repositorios EF Core
      dentro de Infrastructure.
- [ ] Mover el endpoint y la página Razor a `Flights/Presentation`.
- [ ] Crear `FlightsModule` como único punto de registro público.
- [ ] Retirar los proyectos globales `Airport.Core` y `Airport.Infrastructure` cuando
      Flights ya funcione dentro del módulo.
- [ ] Replicar la plantilla sólo al comenzar una feature real; no crear módulos vacíos
      con código ficticio.
- [ ] Crear pruebas unitarias junto a la estructura de cada feature.
- [ ] Añadir pruebas de arquitectura para las reglas de namespace y dependencia.

Esta reorganización se hará sin modificar la etapa 002 ni usar migraciones para cargar
el dump académico.

## Orden funcional posterior

1. Terminar Flights con consulta individual, búsqueda y paginación.
2. Implementar Airports y Airlines para enriquecer la información del vuelo.
3. Implementar Fleet.
4. Implementar Passengers y Bookings.
5. Implementar Weather y Employees si forman parte de los requerimientos evaluados.

No se crearán operaciones CRUD automáticamente para todas las tablas. Cada slice debe
corresponder a un caso de uso solicitado por el examen.

## Criterio de terminado

- Todas las features implementadas contienen Domain, Application, Infrastructure y
  Presentation.
- Los hosts no contienen lógica específica del negocio.
- Cada caso de uso está encapsulado en un vertical slice.
- Domain permanece libre de frameworks y adaptadores.
- PostgreSQL, EF Core, Npgsql y `DbContext` sólo se conocen dentro de Infrastructure.
- Todo acceso de la aplicación a PostgreSQL se realiza mediante EF Core; no se
  introducen consultas directas desde Presentation o Domain.
- Razor sólo se conoce dentro de Presentation y el host Web.
- Las features no se acoplan mediante sus detalles internos.
- La estructura puede entenderse comenzando por nombres del negocio.
