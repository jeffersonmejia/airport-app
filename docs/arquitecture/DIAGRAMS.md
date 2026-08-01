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
    auth -- No --> login[Registrar usuario o iniciar sesión]
    login --> auth
    auth -- Sí --> criteria[Seleccionar origen, destino y fecha]
    criteria --> validCriteria{¿Criterios válidos?}
    validCriteria -- No --> criteriaError[Mostrar validación segura]
    criteriaError --> criteria
    validCriteria -- Sí --> search[Consultar vuelos reales con filtros y paginación física]
    search --> available{¿Hay vuelos disponibles?}
    available -- No --> empty[Mostrar estado sin resultados]
    empty --> criteria
    available -- Sí --> detail[Consultar detalle y seleccionar tarifa]
    detail --> create[Crear orden con identidad de la cookie]
    create --> recalculate[Recalcular tarifa y monto en el servidor]
    recalculate --> validOffer{¿Vuelo y tarifa disponibles?}
    validOffer -- No --> rejected[Rechazar la operación]
    validOffer -- Sí --> pending[Persistir orden PENDING_PAYMENT y su detalle]
    pending --> paypalOrder[Crear orden PayPal con clave de idempotencia]
    paypalOrder --> approval[Redirigir al cliente a PayPal Sandbox]
    approval --> decision{Resultado en PayPal}
    decision -- Cancelado --> cancelled[Mostrar pago cancelado; no emitir boleto]
    decision -- Aprobado --> capture[Solicitar captura desde el backend]
    capture --> verify{¿COMPLETED y coinciden monto y moneda?}
    verify -- No --> failed[Rechazar captura; conservar orden pendiente]
    verify -- Sí --> transaction[Transacción serializable]
    transaction --> payment[Marcar pago COMPLETED]
    payment --> paid[Marcar orden PAID]
    paid --> unique{¿La orden ya tiene boleto?}
    unique -- Sí --> receipt[Recuperar boleto existente]
    unique -- No --> ticket[Emitir un único boleto]
    ticket --> receipt[Mostrar comprobante e historial]
    receipt --> finish([Fin])
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
classDiagram
    direction LR

    class BookingEndpoints {
        +CreateAsync(request, principal)
        +GetAsync(orderId, principal)
        +GetHistoryAsync(page, principal)
        +GetReceiptAsync(orderId, principal)
    }

    class CreateOrderValidator {
        +Validate(command) Errors
    }

    class CreateOrderHandler {
        -IBookingRepository repository
        -TimeProvider timeProvider
        +HandleAsync(command) CreateOrderResponse
    }

    class IBookingRepository {
        <<port>>
        +FindFlightOfferAsync(flightId) FlightOffer
        +AddAsync(order)
        +FindOwnedAsync(orderId, userId) TicketOrder
        +SearchOwnedAsync(userId, page, size) BookingHistoryPage
    }

    class PostgresBookingRepository {
        <<adapter>>
        +FindFlightOfferAsync(flightId) FlightOffer
        +AddAsync(order)
        +FindOwnedAsync(orderId, userId) TicketOrder
        +SearchOwnedAsync(userId, page, size) BookingHistoryPage
    }

    class TicketFare {
        <<domain>>
        +FromFlight(code, departure, arrival) TicketFare
        +Price decimal
    }

    class TicketOrder {
        <<domain>>
        +UserId string
        +Total decimal
        +CurrencyCode string
        +Status string
        +PendingPayment string
        +Paid string
    }

    class CreatePayPalOrderEndpoint {
        +HandleAsync(request, principal)
    }

    class CreatePayPalOrderHandler {
        -IPayPalGateway gateway
        -IPaymentOrderStore store
        +HandleAsync(command) Response
    }

    class CapturePayPalOrderEndpoint {
        +HandleAsync(orderId, request, principal)
    }

    class CapturePayPalOrderHandler {
        -IPayPalGateway gateway
        -IPaymentOrderStore store
        +HandleAsync(command) Response
    }

    class IPaymentOrderStore {
        <<port>>
        +FindPayableAsync(orderId, userId) PayableTicketOrder
        +FindByIdempotencyKeyAsync(key, userId) RecordedPayPalPayment
        +FindByProviderOrderAsync(providerId, userId) RecordedPayPalPayment
        +RecordCreatedAsync(order, providerId, key)
        +CompleteAsync(payment, captureId, amount, currency)
    }

    class PostgresPaymentOrderStore {
        <<adapter>>
        +RecordCreatedAsync(order, providerId, key)
        +CompleteAsync(payment, captureId, amount, currency)
    }

    class IPayPalGateway {
        <<port>>
        +CreateOrderAsync(request) PayPalOrder
        +CaptureOrderAsync(orderId, key) PayPalCapture
    }

    class PayPalPaymentGateway {
        <<adapter>>
        +CreateOrderAsync(request) PayPalOrder
        +CaptureOrderAsync(orderId, key) PayPalCapture
    }

    class BookingsDbContext {
        <<EF Core>>
        +Orders DbSet
        +OrderDetails DbSet
        +Payments DbSet
        +PurchasedTickets DbSet
    }

    class OrderRow {
        +Id Guid
        +UserId string
        +Total decimal
        +Status string
        +Ticket PurchasedTicketRow
        +Payments PaymentRow[]
    }

    class PaymentRow {
        +ProviderOrderId string
        +ProviderCaptureId string
        +IdempotencyKey string
        +Amount decimal
        +CurrencyCode string
        +Status string
    }

    class PurchasedTicketRow {
        +TicketNumber string
        +IssuedAt DateTimeOffset
    }

    BookingEndpoints --> CreateOrderValidator : valida
    BookingEndpoints --> CreateOrderHandler : delega
    CreateOrderHandler --> IBookingRepository : usa
    CreateOrderHandler --> TicketFare : recalcula en servidor
    CreateOrderHandler --> TicketOrder : crea PENDING_PAYMENT
    PostgresBookingRepository ..|> IBookingRepository : implementa
    PostgresBookingRepository --> BookingsDbContext : consulta y persiste

    CreatePayPalOrderEndpoint --> CreatePayPalOrderHandler : delega con userId
    CapturePayPalOrderEndpoint --> CapturePayPalOrderHandler : delega con userId
    CreatePayPalOrderHandler --> IPaymentOrderStore : valida propiedad e idempotencia
    CreatePayPalOrderHandler --> IPayPalGateway : crea solicitud
    CapturePayPalOrderHandler --> IPaymentOrderStore : obtiene pago propio
    CapturePayPalOrderHandler --> IPayPalGateway : captura y verifica
    PostgresPaymentOrderStore ..|> IPaymentOrderStore : implementa
    PayPalPaymentGateway ..|> IPayPalGateway : implementa
    PostgresPaymentOrderStore --> BookingsDbContext : transacción serializable
    BookingsDbContext --> OrderRow
    OrderRow "1" *-- "many" PaymentRow
    OrderRow "1" *-- "0..1" PurchasedTicketRow
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
