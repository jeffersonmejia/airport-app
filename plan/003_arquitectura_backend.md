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

Cada feature activa tendrá proyectos separados para Domain, Application,
Infrastructure y sus dos adaptadores de Presentation: API y Razor. De este modo, las
reglas hexagonales se validan mediante referencias de ensamblado y no dependen sólo de
convenciones de carpetas.

Para cuidar los dos núcleos y 6 GB de RAM, los proyectos se crearán únicamente cuando
se implemente una feature real. No se generarán treinta proyectos vacíos y, cuando se
autorice la compilación, se trabajará sin paralelismo innecesario.

## Estructura objetivo

```text
Airport.sln
src/
  Hosts/
    Airport.Api/                              # Host HTTP y composition root
    Airport.Web/                              # Host Razor/Blazor y shell visual

  BuildingBlocks/
    Airport.SharedKernel/                     # Se crea sólo al existir una abstracción real

  Features/
    Auth/                                      # Esqueleto; todavía no se registra
      Domain/
        Airport.Features.Auth.Domain.csproj
      Application/
        Airport.Features.Auth.Application.csproj
        Login/                                 # Slice futuro
        Logout/                                # Slice futuro
        GetCurrentUser/                        # Slice futuro
      Infrastructure/
        Airport.Features.Auth.Infrastructure.csproj
      Presentation/
        Api/
          Airport.Features.Auth.Presentation.Api.csproj
        Web/
          Airport.Features.Auth.Presentation.Web.csproj

    Flights/
      Domain/
        Airport.Features.Flights.Domain.csproj
        Flight.cs
      Application/
        Airport.Features.Flights.Application.csproj
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
        Airport.Features.Flights.Infrastructure.csproj
        Persistence/
          FlightsDbContext.cs
          FlightRow.cs
          PostgresFlightReader.cs
        DependencyInjection.cs
      Presentation/
        Api/
          Airport.Features.Flights.Presentation.Api.csproj
          FlightsModule.cs
          GetFlight/
            GetFlightEndpoint.cs
          SearchFlights/
            SearchFlightsEndpoint.cs
        Web/
          Airport.Features.Flights.Presentation.Web.csproj
          Pages/
            Flights.razor
          Components/
            FlightCard.razor
            FlightSearch.razor
          Models/
          Services/
          DependencyInjection.cs

    FeatureSiguiente/                         # Sólo cuando exista un caso de uso
      Domain/
      Application/
      Infrastructure/
      Presentation/
        Api/
        Web/

tests/
  Airport.UnitTests/
    Features/
      Flights/
      Bookings/
      Airports/
  Airport.ArchitectureTests/
    DependencyRulesTests.cs
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
7. `Presentation/Api` expone el registro HTTP del módulo y `Presentation/Web` expone
   el registro de sus páginas, componentes y clientes.
8. Las consultas grandes siempre usan paginación, proyección y cancelación.
9. Auth es una feature independiente: Flights y las demás features no implementan
   login, sesiones, autorización ni acceso directo a credenciales.

## Feature Auth

Auth se mantiene como esqueleto hasta definir los requisitos concretos del examen.
Tiene proyectos independientes para Domain, Application, Infrastructure,
Presentation/Api y Presentation/Web, pero todavía no se registra en los hosts.

Slices previstos:

- `Login`.
- `Logout`.
- `GetCurrentUser`.

Antes de implementar comportamiento se decidirán cookies o JWT, duración de sesión,
roles, protección CSRF y política de bloqueo. El campo legado
`airportdb.employee.password` no se tratará como contraseña segura ni se comparará en
texto plano. Auth será responsable de credenciales e identidad; Employees será
responsable de información laboral y personal.

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
| Auth | Proyección de credenciales de `employee`; diseño definitivo pendiente |
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

## Estado de la reorganización

- [x] Crear `src/Hosts` y `src/Features`.
- [x] Mover `Airport.Api` y `Airport.Web` a `src/Hosts`.
- [x] Separar Flights en proyectos Domain, Application, Infrastructure,
      Presentation/Api y Presentation/Web.
- [x] Mover `Flight` a `Flights/Domain`.
- [x] Mover query, validator, handler, response y puerto a `Flights/Application`.
- [x] Mover `FlightsDbContext`, mapeo y lector PostgreSQL a
      `Flights/Infrastructure`.
- [x] Mantener EF Core y Npgsql exclusivamente dentro de Infrastructure.
- [x] Mover endpoint, página y componentes Razor a `Flights/Presentation`.
- [x] Crear registros públicos separados para Presentation/Api y Presentation/Web.
- [x] Implementar búsqueda paginada con proyección, `AsNoTracking` y cancelación.
- [x] Incorporar listado y navegación de páginas en la presentación Razor.
- [x] Retirar los proyectos globales `Airport.Core` y `Airport.Infrastructure`.
- [x] Reubicar las pruebas unitarias bajo `Features/Flights` sin ejecutarlas.
- [x] Crear el esqueleto independiente de Auth con las cinco fronteras de proyecto.
- [x] Documentar los slices futuros y decisiones de seguridad pendientes de Auth.
- [x] No crear SharedKernel todavía: no existe una abstracción compartida real y se
      evita introducir acoplamiento prematuro.
- [x] Añadir pruebas de arquitectura para las reglas de dependencia, sin ejecutarlas.
- [x] Centralizar nullable, implicit usings y análisis estático en
      `Directory.Build.props`.
- [ ] Restaurar paquetes, compilar y ejecutar pruebas cuando se autorice.

Esta reorganización no modificó la etapa 002 ni utilizó migraciones para cargar el
dump académico.

## Orden funcional posterior

1. Flights ya cuenta con consulta individual, búsqueda y paginación; falta validar
   su ejecución cuando se autoricen restore, build y acceso a la base.
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
- Auth permanece aislado y sin comportamiento ficticio hasta definir sus requisitos
  de seguridad.
