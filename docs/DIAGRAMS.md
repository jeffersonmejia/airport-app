# Diagramas C4 — Airport

Este documento describe la arquitectura implementada actualmente. Los diagramas usan
la jerarquía C4 como guía y añaden vistas de apoyo cuando una relación no pertenece
estrictamente a un nivel C4.

## Criterio de documentación

En un proyecto profesional no se crean automáticamente los cuatro niveles C4 para
cada módulo. Se mantiene una sola vista de contexto del sistema y una vista de
contenedores; después se agregan diagramas de componentes sólo para los contenedores
o módulos cuya estructura necesite explicación. El nivel de código se reserva para
flujos críticos o patrones de referencia.

En Airport se documentan los hosts completos y se usa Flights como vertical slice de
referencia. Las features futuras deben seguir el mismo patrón sin duplicar diagramas
hasta que introduzcan una decisión arquitectónica distinta.

## Nivel 1 — Contexto del sistema

```mermaid
flowchart LR
    traveler["Usuario<br/>Consulta información de vuelos"]
    operator["Responsable del entorno<br/>Configura y valida la aplicación"]
    source["Fuente de datos del examen<br/>Dump PostgreSQL 18.4"]

    airport["Sistema Airport<br/>Aplicación web para consultar datos aeroportuarios"]

    traveler -->|Usa desde el navegador| airport
    operator -->|Configura, restaura y supervisa| airport
    source -->|Carga inicial controlada| airport

    classDef person fill:#08427b,color:#fff,stroke:#052e56
    classDef system fill:#1168bd,color:#fff,stroke:#0b4884
    classDef external fill:#999,color:#fff,stroke:#666
    class traveler,operator person
    class airport system
    class source external
```

## Nivel 2 — Contenedores

```mermaid
flowchart LR
    user["Usuario"]
    operator["Responsable del entorno"]
    dump[("aeropuerto-db.sql<br/>Dump externo")]

    subgraph airport["Sistema Airport"]
        web["Airport.Web<br/>Blazor WebAssembly .NET 10<br/>Shell y UI Material Design"]
        api["Airport.Api<br/>ASP.NET Core .NET 10<br/>API HTTP y composition root"]
        db[("aereopuerto_db<br/>Esquema airportdb<br/>PostgreSQL 18")]
    end

    psql["psql<br/>Herramienta local de restauración"]
    secrets[".NET User Secrets<br/>Configuración local fuera de Git"]

    user -->|HTTPS| web
    web -->|HTTP/JSON| api
    api -->|EF Core + Npgsql| db
    operator -->|Ejecuta| psql
    dump -->|SQL plano| psql
    psql -->|Restaura| db
    secrets -->|Conexión y parámetros JWT| api

    classDef person fill:#08427b,color:#fff,stroke:#052e56
    classDef container fill:#438dd5,color:#fff,stroke:#1d5f9a
    classDef database fill:#438dd5,color:#fff,stroke:#1d5f9a
    classDef external fill:#999,color:#fff,stroke:#666
    class user,operator person
    class web,api container
    class db database
    class dump,psql,secrets external
```

La restauración con `psql` prepara la base, pero no forma parte del runtime. La
aplicación accede a PostgreSQL exclusivamente mediante EF Core y Npgsql.

## Nivel 3A — Componentes de Airport.Api

```mermaid
flowchart LR
    client["Airport.Web u otro cliente HTTP"]

    subgraph api["Contenedor Airport.Api"]
        host["Program.cs<br/>Composition root"]
        pipeline["Pipeline ASP.NET Core<br/>HTTPS, CORS, autenticación y autorización"]

        subgraph flights["Feature Flights"]
            flightsApi["Presentation.Api<br/>Endpoints GetFlight y SearchFlights"]
            flightsApp["Application<br/>Queries, validadores, handlers y puertos"]
            flightsDomain["Domain<br/>Flight"]
            flightsInfra["Infrastructure<br/>CachedFlightReader, EF Core y Npgsql"]
        end

        subgraph auth["Feature Auth"]
            authApi["Presentation.Api<br/>JWT Bearer y validación de sesión"]
            authApp["Application<br/>Puertos de token y sesión"]
            authDomain["Domain<br/>AuthIdentity"]
            authInfra["Infrastructure<br/>Emisión JWT y sesión activa"]
        end

        cache["Airport.Caching<br/>Caché en memoria limitada"]
        policies["SharedKernel<br/>Paginación y políticas de caché"]
    end

    db[("PostgreSQL<br/>aereopuerto_db / airportdb")]

    client -->|HTTP/JSON| pipeline
    host -->|Registra| pipeline
    host -->|Registra módulos| flightsApi
    host -->|Registra módulos| authApi
    pipeline --> flightsApi
    pipeline --> authApi

    flightsApi --> flightsApp
    flightsApp --> flightsDomain
    flightsInfra -. implementa puertos .-> flightsApp
    flightsInfra --> flightsDomain
    flightsInfra --> cache
    flightsInfra -->|EF Core + Npgsql| db
    flightsApp --> policies

    authApi --> authApp
    authApp --> authDomain
    authInfra -. implementa puertos .-> authApp
    authInfra --> authDomain
    authInfra --> cache

    classDef adapter fill:#438dd5,color:#fff,stroke:#1d5f9a
    classDef core fill:#85bbf0,color:#111,stroke:#1d5f9a
    classDef shared fill:#b7d8f7,color:#111,stroke:#1d5f9a
    classDef database fill:#438dd5,color:#fff,stroke:#1d5f9a
    class host,pipeline,flightsApi,flightsInfra,authApi,authInfra adapter
    class flightsApp,flightsDomain,authApp,authDomain core
    class cache,policies shared
    class db database
```

Las flechas punteadas representan inversión de dependencias: Infrastructure
implementa puertos definidos por Application. Domain y Application no conocen EF
Core, Npgsql ni ASP.NET Core.

Auth configura emisión y validación JWT, vida de 15 minutos, clock skew de 30
segundos y una sesión activa por usuario mediante `sub` y `jti`. Los slices de
Login, Logout y CurrentUser todavía no están implementados.

## Nivel 3B — Componentes de Airport.Web

```mermaid
flowchart LR
    browser["Navegador"]

    subgraph web["Contenedor Airport.Web"]
        bootstrap["Program.cs + App.razor<br/>Arranque y rutas"]
        shell["Layout global<br/>MainLayout y NavMenu"]
        globalCss["Estilos globales<br/>Tokens Material, base, shell y estados"]

        subgraph flightsWeb["Flights.Presentation.Web"]
            page["Flights.razor<br/>Orquestación de la pantalla"]
            search["FlightSearch<br/>Formulario de búsqueda"]
            card["FlightCard<br/>Detalle del vuelo"]
            list["FlightListItem<br/>Listado paginado"]
            client["FlightsClient<br/>Adaptador HTTP"]
            featureCss["Estilos de Flights<br/>Componentes, estados y responsive"]
        end
    end

    api["Airport.Api"]

    browser --> bootstrap
    bootstrap --> shell
    shell --> page
    page --> search
    page --> card
    page --> list
    page --> client
    client -->|HTTP/JSON| api
    globalCss -->|Tokens compartidos| featureCss
    featureCss --> search
    featureCss --> card
    featureCss --> list

    classDef host fill:#438dd5,color:#fff,stroke:#1d5f9a
    classDef feature fill:#85bbf0,color:#111,stroke:#1d5f9a
    classDef external fill:#999,color:#fff,stroke:#666
    class bootstrap,shell,globalCss host
    class page,search,card,list,client,featureCss feature
    class browser,api external
```

El host Web sólo conserva el arranque, el shell y los estilos transversales. La
página, los componentes, el cliente HTTP y los estilos específicos viven dentro de
`Flights/Presentation/Web`.

## Nivel 4 — Código del slice GetFlight

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
        +Validate(query) string
    }
    class GetFlightHandler {
        -IFlightReader flightReader
        +HandleAsync(query, cancellationToken) GetFlightResponse
    }
    class IFlightReader {
        <<port>>
        +FindByIdAsync(id, cancellationToken) Flight
    }
    class CachedFlightReader {
        <<decorator>>
        +FindByIdAsync(id, cancellationToken) Flight
    }
    class PostgresFlightReader {
        <<adapter>>
        +FindByIdAsync(id, cancellationToken) Flight
    }
    class FlightsDbContext {
        <<EF Core>>
    }
    class Flight {
        <<domain>>
    }
    class GetFlightResponse {
        <<contract>>
    }

    GetFlightEndpoint --> GetFlightQuery : crea
    GetFlightEndpoint --> GetFlightValidator : valida
    GetFlightEndpoint --> GetFlightHandler : delega
    GetFlightHandler --> IFlightReader : usa
    GetFlightHandler --> GetFlightResponse : devuelve
    CachedFlightReader ..|> IFlightReader : decora
    CachedFlightReader --> PostgresFlightReader : delega en
    PostgresFlightReader ..|> IFlightReader : implementa
    PostgresFlightReader --> FlightsDbContext : consulta
    PostgresFlightReader --> Flight : proyecta
```

Este slice sirve como patrón para operaciones futuras: Presentation valida y delega,
Application coordina mediante puertos, Infrastructure implementa adaptadores y Domain
permanece libre de frameworks.

## Vista de apoyo — Políticas transversales

```mermaid
flowchart LR
    request["Solicitud HTTP"] --> auth["JWT Bearer<br/>Firma, issuer, audience y expiración"]
    auth --> session["Sesión activa<br/>sub + jti"]
    session --> validation["Validación del caso de uso"]
    validation --> pagination["Paginación<br/>máximo 5 elementos"]
    pagination --> cache["Caché de lectura<br/>TTL 30 s / 256 entradas"]
    cache --> database[("PostgreSQL")]

    secrets["User Secrets<br/>Conexión + JWT"] --> auth
    secrets --> database
```

User Secrets se usa sólo para desarrollo local y no se versiona. Caché, paginación,
CORS y demás límites operativos son políticas o configuración, no secretos.

## Vista de apoyo — Features y propiedad de tablas

```mermaid
flowchart TB
    Flights["Flights — implementada"] --> flight
    Flights --> flight_log
    Flights --> flightschedule

    Auth["Auth — infraestructura JWT;<br/>persistencia pendiente"] -.-> employee

    Airports["Airports — futura"] -.-> airport
    Airports -.-> airport_geo
    Airports -.-> airport_reachable

    Airlines["Airlines — futura"] -.-> airline
    Fleet["Fleet — futura"] -.-> airplane
    Fleet -.-> airplane_type

    Bookings["Bookings — futura"] -.-> booking
    Passengers["Passengers — futura"] -.-> passenger
    Passengers -.-> passengerdetails
    Employees["Employees — futura"] -.-> employee
    Weather["Weather — futura"] -.-> weatherdata
```

Las líneas continuas representan responsabilidad implementada; las discontinuas,
responsabilidad planificada. Una relación SQL no autoriza a una feature a acceder a
los detalles internos de otra.
