# Diagramas C4 — Airport

Estos diagramas describen la arquitectura objetivo. El proyecto todavía no ha sido
generado y algunos nombres podrán ajustarse a los endpoints definitivos del examen.

## Nivel 1 — Contexto del sistema

```mermaid
flowchart LR
    user["Usuario / sistema cliente<br/>Consulta vuelos, aeropuertos y reservas"]
    admin["Administrador académico<br/>Opera y verifica la solución"]
    source["Fuente de datos del examen<br/>Dump PostgreSQL 18.4"]

    system["Sistema Airport<br/>API de gestión aeroportuaria y su base de datos"]

    user -->|HTTPS / JSON| system
    admin -->|Administra y valida| system
    source -->|Importación inicial controlada| system

    classDef person fill:#08427b,color:#fff,stroke:#052e56
    classDef system fill:#1168bd,color:#fff,stroke:#0b4884
    classDef external fill:#999,color:#fff,stroke:#666
    class user,admin person
    class system system
    class source external
```

## Nivel 2 — Contenedores

```mermaid
flowchart LR
    user["Usuario / cliente"]
    admin["Administrador"]
    dump[("aeropuerto-db.sql<br/>2.0 GiB")]

    subgraph airport["Sistema Airport"]
        api["Airport.Api<br/>ASP.NET Core .NET 10<br/>Expone HTTP y compone dependencias"]
        importer["Importador reanudable<br/>Proceso local secuencial<br/>Divide COPY en ciclos"]
        db[("airport_exam / airportdb<br/>PostgreSQL 18 nativo<br/>Datos aeroportuarios")]
        state[("Estado local de importación<br/>Manifest + checkpoints")]
    end

    user -->|HTTPS / JSON| api
    admin -->|Ejecuta y supervisa| importer
    dump -->|Lectura streaming| importer
    importer -->|COPY por bloques| db
    importer -->|Actualiza al confirmar cada bloque| state
    api -->|Npgsql / SQL| db

    classDef person fill:#08427b,color:#fff,stroke:#052e56
    classDef container fill:#438dd5,color:#fff,stroke:#1d5f9a
    classDef database fill:#438dd5,color:#fff,stroke:#1d5f9a
    classDef external fill:#999,color:#fff,stroke:#666
    class user,admin person
    class api,importer container
    class db,state database
    class dump external
```

## Nivel 3 — Componentes de Airport.Api

```mermaid
flowchart LR
    client["Cliente HTTP"]

    subgraph api["Airport.Api — adaptador de entrada y composición"]
        middleware["Pipeline HTTP<br/>Errores, logging, auth y validación"]
        endpoints["Endpoints por feature<br/>Flights, Bookings, Airports,<br/>Passengers, Airlines, Weather"]
        di["Composition Root<br/>Registro de puertos y adaptadores"]
    end

    subgraph core["Airport.Core — hexágono"]
        slices["Vertical slices<br/>Request + Validator + Handler + Response"]
        domain["Modelo de dominio<br/>Entidades, value objects y reglas"]
        ports["Puertos de salida<br/>Repositorios, Unit of Work, Clock"]
    end

    subgraph infra["Airport.Infrastructure — adaptadores de salida"]
        persistence["Persistencia por feature<br/>Repositorios / consultas Npgsql"]
        mapping["Mapeo PostgreSQL<br/>airportdb.*"]
        observability["Logging y métricas"]
    end

    db[("PostgreSQL<br/>airport_exam")]

    client -->|HTTPS| middleware
    middleware --> endpoints
    endpoints --> slices
    slices --> domain
    slices --> ports
    persistence -. implementa .-> ports
    persistence --> mapping
    mapping -->|SQL| db
    di -. ensambla .-> endpoints
    di -. ensambla .-> persistence
    middleware --> observability

    classDef adapter fill:#438dd5,color:#fff,stroke:#1d5f9a
    classDef core fill:#85bbf0,color:#111,stroke:#1d5f9a
    classDef db fill:#438dd5,color:#fff,stroke:#1d5f9a
    class middleware,endpoints,di,persistence,mapping,observability adapter
    class slices,domain,ports core
    class db db
```

La flecha punteada desde Persistence hacia Ports representa inversión de dependencia:
Infrastructure conoce las interfaces del Core; el Core no conoce Infrastructure.

## Nivel 4 — Código de un vertical slice de ejemplo

```mermaid
classDiagram
    direction LR

    class GetFlightEndpoint {
        +HandleAsync(id, cancellationToken)
    }
    class GetFlightQuery {
        +int FlightId
    }
    class GetFlightValidator {
        +Validate(query)
    }
    class GetFlightHandler {
        -IFlightReader reader
        +Handle(query, cancellationToken) FlightResponse
    }
    class IFlightReader {
        <<port>>
        +FindById(id, cancellationToken) Flight
    }
    class PostgresFlightReader {
        <<adapter>>
        +FindById(id, cancellationToken) Flight
    }
    class Flight {
        <<aggregate>>
        +FlightId
        +FlightNumber
        +Departure
        +Arrival
    }
    class FlightResponse {
        +FlightId
        +FlightNumber
        +Departure
        +Arrival
    }
    class AirportDbContext {
        <<PostgreSQL mapping>>
    }

    GetFlightEndpoint --> GetFlightQuery : crea
    GetFlightEndpoint --> GetFlightValidator : valida con
    GetFlightEndpoint --> GetFlightHandler : delega
    GetFlightHandler --> IFlightReader : usa puerto
    GetFlightHandler --> Flight : obtiene
    GetFlightHandler --> FlightResponse : proyecta
    PostgresFlightReader ..|> IFlightReader : implementa
    PostgresFlightReader --> AirportDbContext : consulta
```

El mismo patrón se repetirá dentro de cada feature. Una operación de escritura puede
añadir un puerto `IUnitOfWork` y reglas del agregado, pero no introducirá una capa
global de servicios que mezcle casos de uso.
## Mapa de features hacia las tablas existentes

```mermaid
flowchart TB
    Airports["Airports"] --> airport
    Airports --> airport_geo
    Airports --> airport_reachable

    Airlines["Airlines y Fleet"] --> airline
    Airlines --> airplane
    Airlines --> airplane_type

    Flights["Flights"] --> flight
    Flights --> flight_log
    Flights --> flightschedule

    Bookings["Bookings y Passengers"] --> booking
    Bookings --> passenger
    Bookings --> passengerdetails

    Operations["Operations"] --> employee
    Operations --> weatherdata
```
