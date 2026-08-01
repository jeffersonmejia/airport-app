# Diagramas del sistema Airport — Tipo 1: Compra de boletos

Este documento presenta las vistas estructurales y de comportamiento del sistema implementado. La
numeración continúa la secuencia del informe técnico. El mayor nivel de detalle se concentra en la
compra y el procesamiento de pagos porque la integración funcional con la pasarela representa el
criterio de mayor ponderación de la rúbrica: 5 puntos. También se hacen visibles la autorización,
el aislamiento por usuario, el cálculo de tarifas en el servidor, la idempotencia y la emisión única
del boleto.

## 4. Diagrama de entidad-relación de la base de datos

Las tablas originales se conservan en `airportdb`. Las tablas creadas para el ejercicio y las de
ASP.NET Core Identity se aíslan en `airport_app`. Las relaciones marcadas como lógicas se validan
desde la aplicación; no representan claves foráneas físicas entre contextos.

```mermaid
erDiagram
    AIRPORT_AIRPORT {
        int airport_id PK
        string iata
        string icao
        string name
    }

    AIRPORT_FLIGHT {
        int flight_id PK
        int from_airport_id FK
        int to_airport_id FK
        string flightno
        datetime departure
        datetime arrival
    }

    IDENTITY_USERS {
        string Id PK
        string UserName
        string Email
        bool EmailConfirmed
        string PasswordHash
    }

    IDENTITY_ROLES {
        string Id PK
        string Name
    }

    IDENTITY_USER_ROLES {
        string UserId PK,FK
        string RoleId PK,FK
    }

    ORDERS {
        uuid order_id PK
        string user_id
        int flight_id
        string flight_number
        string origin_code
        string destination_code
        datetime departure
        string fare_code
        decimal total
        string currency_code
        string status
        datetime created_at
    }

    ORDER_DETAILS {
        uuid order_detail_id PK
        uuid order_id FK
        string passenger_first_name
        string passenger_last_name
        string passport_number
        int quantity
        decimal unit_price
    }

    PAYMENTS {
        uuid payment_id PK
        uuid order_id FK
        string provider
        string provider_order_id UK
        string provider_capture_id UK
        string idempotency_key UK
        string status
        decimal amount
        string currency_code
        datetime created_at
        datetime completed_at
    }

    PURCHASED_TICKETS {
        uuid purchased_ticket_id PK
        uuid order_id FK,UK
        int flight_id
        string ticket_number UK
        string fare_code
        datetime issued_at
    }

    AIRPORT_AIRPORT ||--o{ AIRPORT_FLIGHT : origen
    AIRPORT_AIRPORT ||--o{ AIRPORT_FLIGHT : destino
    AIRPORT_FLIGHT ||--o{ ORDERS : "referencia lógica"
    IDENTITY_USERS ||--o{ IDENTITY_USER_ROLES : asigna
    IDENTITY_ROLES ||--o{ IDENTITY_USER_ROLES : contiene
    IDENTITY_USERS ||--o{ ORDERS : "propiedad lógica"
    ORDERS ||--|| ORDER_DETAILS : detalla
    ORDERS ||--o{ PAYMENTS : registra
    ORDERS ||--o| PURCHASED_TICKETS : emite
```

## 5. Diagrama de arquitectura

La solución combina arquitectura hexagonal, vertical slices y screaming architecture. Cada feature
expone casos de uso desde Application, mantiene reglas puras en Domain y accede a servicios externos
mediante puertos implementados por Infrastructure.

```mermaid
flowchart LR
    browser["Navegador del cliente o administrador"]

    subgraph presentation["Presentation"]
        web["Airport.Web<br/>Blazor WebAssembly"]
        api["Airport.Api<br/>Minimal APIs"]
    end

    subgraph features["Features del negocio"]
        direction TB
        auth["Auth<br/>registro, sesión, roles y MFA"]
        flights["Flights<br/>búsqueda, filtros, detalle y paginación"]
        bookings["Bookings<br/>orden, historial, boleto y comprobante"]
        payments["Payments<br/>creación y captura PayPal"]
        administration["Administration<br/>consulta global protegida"]
    end

    subgraph hexagon["Núcleo hexagonal de cada feature"]
        app["Application<br/>slices, validadores y puertos"]
        domain["Domain<br/>reglas y estados"]
        infra["Infrastructure<br/>adaptadores EF Core, Identity y PayPal"]
    end

    postgres[("PostgreSQL<br/>airportdb + airport_app")]
    paypal["PayPal Sandbox"]
    google["Google OAuth"]
    smtp["Servidor SMTP"]

    browser --> web
    web -->|HTTPS y JSON| api
    api --> auth
    api --> flights
    api --> bookings
    api --> payments
    api --> administration
    auth --> app
    flights --> app
    bookings --> app
    payments --> app
    administration --> app
    app --> domain
    infra -. implementa puertos .-> app
    infra --> postgres
    infra --> paypal
    infra --> google
    infra --> smtp
```

## 6. Diagrama de flujo del proceso de compra

```mermaid
flowchart TD
    start([Inicio]) --> auth{¿Cliente autenticado?}
    auth -- No --> login[Registrarse o iniciar sesión]
    login --> auth
    auth -- Sí --> search[Buscar vuelo y seleccionar tarifa]
    search --> order[Recalcular monto y crear orden pendiente]
    order --> validOrder{¿Orden válida?}
    validOrder -- No --> rejected[Rechazar operación]
    validOrder -- Sí --> paypal[Procesar pago en PayPal Sandbox]
    paypal --> approved{¿Pago aprobado?}
    approved -- No --> cancelled[Cancelar sin emitir boleto]
    approved -- Sí --> verify[Verificar captura, monto y moneda]
    verify --> validPayment{¿Captura válida?}
    validPayment -- No --> failed[Rechazar pago]
    validPayment -- Sí --> complete[Confirmar compra y emitir un único boleto]
    complete --> receipt[Mostrar comprobante e historial]
    rejected --> finish([Fin])
    cancelled --> finish
    failed --> finish
    receipt --> finish
```

## 7. Diagrama de casos de uso

```mermaid
flowchart LR
    visitor([Visitante])
    client([Cliente])
    admin([Administrador])
    paypal([PayPal Sandbox])

    subgraph airport["Sistema Airport"]
        register((Registrarse))
        login((Iniciar sesión))
        search((Buscar vuelos))
        filter((Filtrar y paginar))
        detail((Consultar detalle y tarifa))
        order((Crear orden))
        pay((Pagar orden))
        verify((Verificar captura))
        receipt((Consultar boleto y comprobante))
        history((Consultar historial propio))
        global((Consultar órdenes y pagos globales))
        logout((Cerrar sesión))
    end

    visitor --> register
    visitor --> login
    visitor --> search
    visitor --> filter
    visitor --> detail
    client --> search
    client --> filter
    client --> detail
    client --> order
    client --> pay
    client --> receipt
    client --> history
    client --> logout
    admin --> global
    admin --> logout
    pay --> verify
    paypal --> verify
```

## 8. Diagrama de secuencia del pago

Este flujo destaca la integración de mayor valor en la rúbrica: el navegador no confirma pagos ni
define importes; el backend recupera la orden del usuario, crea la solicitud, verifica la captura y
persiste el resultado de forma idempotente.

```mermaid
sequenceDiagram
    autonumber
    actor C as Cliente
    participant W as Airport.Web
    participant API as Airport.Api
    participant P as PayPal Sandbox
    participant DB as PostgreSQL

    C->>W: Pagar orden
    W->>API: Solicitar pago de la orden
    API->>DB: Validar propietario, estado e idempotencia
    DB-->>API: Orden pendiente con monto y moneda
    API->>P: Crear orden PayPal
    P-->>API: URL de aprobación
    API-->>W: URL de aprobación
    W-->>C: Redirigir a PayPal
    C->>P: Aprobar pago
    P-->>W: Retornar a la aplicación
    W->>API: Solicitar captura
    API->>P: Capturar y consultar resultado
    P-->>API: Estado, monto, moneda y captureId

    alt Captura inválida
        API-->>W: Rechazar pago y no emitir boleto
    else Captura válida
        API->>DB: Registrar pago y emitir un único boleto
        DB-->>API: Compra confirmada
        API-->>W: Comprobante
        W-->>C: Mostrar boleto e historial
    end
```

## 9. Diagrama C4 de Contexto

```mermaid
flowchart LR
    visitor["Visitante<br/>consulta vuelos"]
    client["Cliente<br/>compra y consulta sus boletos"]
    admin["Administrador<br/>revisa operaciones globales"]
    airport["Sistema Airport<br/>Compra de boletos con datos aeroportuarios reales"]
    paypal["PayPal Sandbox<br/>autoriza y captura pagos"]
    google["Google Identity<br/>autenticación externa"]
    smtp["Servicio de correo<br/>confirmación de cuentas"]
    postgres["PostgreSQL Airport<br/>datos originales y persistencia propia"]

    visitor -->|Busca y filtra vuelos| airport
    client -->|Crea órdenes, paga y consulta historial| airport
    admin -->|Consulta órdenes, pagos y boletos| airport
    airport -->|Crea y verifica pagos| paypal
    airport -->|Autentica clientes| google
    airport -->|Envía confirmaciones| smtp
    airport -->|Lee vuelos y persiste compras| postgres
```

## 10. Diagrama C4 de Contenedores

```mermaid
flowchart LR
    user["Visitante, cliente o administrador"]

    subgraph system["Sistema Airport"]
        web["Airport.Web<br/>Blazor WebAssembly .NET 10<br/>UI, navegación y clientes HTTP"]
        api["Airport.Api<br/>ASP.NET Core .NET 10<br/>autenticación, autorización y casos de uso"]
        cache["Caché en memoria<br/>lecturas limitadas y resumen administrativo"]
        db[("PostgreSQL 18<br/>airportdb: datos originales<br/>airport_app: Identity y compras")]
    end

    paypal["PayPal Sandbox API"]
    google["Google OAuth 2.0"]
    smtp["Servidor SMTP"]
    secrets[".NET User Secrets"]

    user -->|HTTPS| web
    web -->|HTTP/JSON y cookie segura| api
    api -->|EF Core + Npgsql| db
    api --> cache
    api -->|OAuth y captura| paypal
    api -->|OpenID/OAuth| google
    api -->|SMTP| smtp
    secrets -->|conexión, JWT, PayPal, Google y correo| api
```

## 11. Diagrama C4 de Componentes

```mermaid
flowchart TB
    web["Airport.Web"]

    subgraph api["Contenedor Airport.Api"]
        pipeline["Pipeline ASP.NET Core<br/>errores seguros, CORS, cookies/JWT y autorización"]
        auth["Auth<br/>registro, login, logout, sesión, roles y MFA"]
        flights["Flights<br/>SearchFlights, GetFlight y opciones de filtros"]
        bookings["Bookings<br/>CreateOrder, GetOrder, GetHistory y GetReceipt"]
        payments["Payments<br/>CreatePayPalOrder y CapturePayPalOrder"]
        administration["Administration<br/>resumen de base y operaciones comerciales"]
        bookingPorts["Puertos de compra<br/>IBookingRepository e IPaymentOrderStore"]
        paypalPort["Puerto de pasarela<br/>IPayPalGateway"]
        persistence["Adaptadores PostgreSQL<br/>DbContext y repositorios EF Core"]
        gateway["Adaptador PayPal<br/>tokens, creación y captura"]
    end

    db[("PostgreSQL")]
    paypal["PayPal Sandbox"]

    web -->|HTTP/JSON| pipeline
    pipeline --> auth
    pipeline --> flights
    pipeline --> bookings
    pipeline --> payments
    pipeline --> administration
    bookings --> bookingPorts
    payments --> bookingPorts
    payments --> paypalPort
    persistence -. implementa .-> bookingPorts
    gateway -. implementa .-> paypalPort
    persistence --> db
    gateway --> paypal
    administration --> persistence
    flights --> persistence
    auth --> persistence
```

## 12. Diagrama C4 de Código del Módulo de Compra de Boletos y Procesamiento de Pagos

Esta vista recibe prioridad porque reúne las reglas con mayor impacto en la evaluación: usuario
autenticado, propiedad de la orden, precio autoritativo, pago Sandbox real, verificación desde el
backend, idempotencia, transacción y emisión única del boleto.

```mermaid
flowchart LR
    subgraph presentation["Presentation"]
        bookingApi["BookingEndpoints"]
        paymentApi["CreatePayPalOrderEndpoint<br/>CapturePayPalOrderEndpoint"]
    end

    subgraph application["Application"]
        createOrder["CreateOrderHandler"]
        paymentHandlers["CreatePayPalOrderHandler<br/>CapturePayPalOrderHandler"]
        dbPorts["Puertos PostgreSQL<br/>IBookingRepository<br/>IPaymentOrderStore"]
        paypalPort["Puerto de pago<br/>IPayPalGateway"]
    end

    subgraph domain["Domain"]
        rules["TicketFare y TicketOrder<br/>Tarifa calculada en servidor"]
    end

    subgraph infrastructure["Infrastructure"]
        postgresAdapters["PostgresBookingRepository<br/>PostgresPaymentOrderStore"]
        paypalAdapter["PayPalPaymentGateway"]
        context["BookingsDbContext"]
    end

    db[("PostgreSQL<br/>órdenes, pagos y boletos")]
    paypal["PayPal Sandbox"]

    bookingApi --> createOrder
    paymentApi --> paymentHandlers
    createOrder --> rules
    createOrder --> dbPorts
    paymentHandlers -->|propiedad e idempotencia| dbPorts
    paymentHandlers -->|crear y verificar captura| paypalPort
    postgresAdapters -. implementan .-> dbPorts
    paypalAdapter -. implementa .-> paypalPort
    postgresAdapters --> context
    context -->|transacción y boleto único| db
    paypalAdapter -->|crear y capturar| paypal
```

### Trazabilidad de los criterios prioritarios

| Criterio evaluado | Evidencia en los diagramas |
|---|---|
| PayPal Sandbox funcional y verificado por el backend | 6, 8, 10, 11 y 12 |
| Identity, sesiones, roles y autorización | 5, 7, 9, 10 y 11 |
| Flujo completo de compra | 4, 6, 7, 8 y 12 |
| LINQ, filtros y paginación física | 5, 6 y 11 |
| EF Core, relaciones y migraciones propias | 4, 5, 10 y 12 |
| Idempotencia, aislamiento y emisión única | 4, 6, 8 y 12 |
