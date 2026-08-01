# Airport

Sistema web para la búsqueda de vuelos, reserva y compra de boletos aéreos con pago electrónico. Incluye autenticación de usuarios (cuenta local, inicio de sesión con Google y autenticación multifactor), emisión de boletos y comprobantes, así como un panel administrativo para consultar el estado de la base de datos y el comercio.

---

# Tabla de Contenido

1. Introducción
2. Estado del Proyecto
3. Características
4. Tecnologías Utilizadas
5. Arquitectura
6. Estructura del Proyecto
7. Requisitos Previos
8. Instalación
9. Configuración
10. Variables de Entorno
11. Base de Datos
12. Ejecución
13. API
14. Pruebas
15. Seguridad
16. Rendimiento
17. Versionado
18. Autores
19. Contacto

---

# 1. Introducción

Airport es una aplicación orientada a la venta de boletos aéreos en línea. El usuario puede explorar el catálogo de vuelos de una base de datos de aeropuertos, elegir una tarifa, registrar los datos del pasajero y completar la compra mediante la pasarela de pagos de **PayPal en modo Sandbox**. Una vez confirmado el pago, la aplicación emite el boleto y el comprobante de la reserva.

El proyecto resuelve el problema de gestionar todo el ciclo de una compra aérea —autenticación, búsqueda, reserva, pago y comprobante— en un solo sistema, separado por *features* con una arquitectura hexagonal que mantiene la lógica de negocio desacoplada de los adaptadores externos (base de datos, PayPal, Google y correo).

# 2. Estado del Proyecto

- **En desarrollo**: las funcionalidades principales están implementadas y verificadas contra PayPal Sandbox; se continúa puliendo el flujo de pago y la experiencia de usuario.

# 3. Características

- Registro e inicio de sesión con cuenta local, confirmación de correo y recuperación de sesión.
- Inicio de sesión y registro con **Google OAuth 2.0**.
- Autenticación multifactor (MFA/TOTP) con aplicación autenticadora.
- Catálogo de vuelos con búsqueda y filtros (origen, destino, fecha, tarifa).
- Reserva de boletos con datos del pasajero (pasaporte, tarifa).
- Pago con **PayPal Sandbox**: creación de orden y captura del pago.
- Emisión de boleto y comprobante de la compra (historial del cliente).
- Panel administrativo con resumen de la base de datos y del comercio.

# 4. Tecnologías Utilizadas

| Tecnología | Versión | Propósito |
| ---------- | ------- | --------- |
| .NET | 10.0 | Plataforma de ejecución y compilación |
| ASP.NET Core | 10.0 | API REST (Minimal APIs) y servidor HTTP |
| Blazor WebAssembly | 10.0 | Interfaz de usuario en el navegador |
| EF Core (Npgsql) | 10.0 | Mapeo objeto-relacional hacia PostgreSQL |
| PostgreSQL | 18 | Motor de base de datos |
| PayPal REST API | v2 | Pasarela de pagos (Sandbox) |
| Google OAuth 2.0 | — | Autenticación externa |
| ASP.NET Core Identity | 10.0 | Gestión de usuarios, roles y cookies |
| JSON Web Tokens (JWT) | — | Autenticación de la API |

# 5. Arquitectura

## Descripción de la Arquitectura

El proyecto usa **arquitectura hexagonal por features**: cada funcionalidad (`Flights`, `Auth`, `Bookings`, `Payments`, `Administration`) se organiza en las capas **Application** (casos de uso y puertos), **Domain** (entidades y reglas de negocio) e **Infrastructure** (adaptadores concretos: EF Core, Identity, PayPal, Google, SMTP), expuesta a través de la capa **Presentation** (endpoints API y páginas Blazor).

Los *hosts* (`Airport.Api` y `Airport.Web`) componen los módulos: la API registra los *modules* de cada feature y el Web registra sus páginas Blazor. Los adaptadores externos (PayPal, Google, SMTP, base de datos) se inyectan a través de puertos/interfaces definidos en la capa de aplicación, por lo que la lógica de negocio no depende de implementaciones concretas.

## Diagrama de Arquitectura

Los diagramas de arquitectura y de flujo de compra se encuentran en [`docs/ARQUITECTURE.md`](docs/ARQUITECTURE.md).

> **Figura 1.** Arquitectura general del sistema (ver `docs/ARQUITECTURE.md`).

## Diagrama C4

El sistema se documenta con diagramas C4:

- Nivel 1: Contexto (usuarios, sistema, PayPal Sandbox y servicios externos).
- Nivel 2: Contenedores (Web Blazor WASM, API, base de datos, correo).
- Nivel 3: Componentes (módulos por feature y sus puertos/adaptadores).
- Nivel 4: Código del flujo de pago (creación y captura de la orden PayPal).

## Código Relevante

El fragmento más relevante es la **captura del pago**, donde la pasarela confirma el cobro y la aplicación persiste la orden, el pago y el boleto de forma transaccional:

```csharp
var capture = await gateway.CaptureOrderAsync(orderId, idempotencyKey, ct);
var capturedAmount = capture.CapturedAmount;

if (capture.Status != "COMPLETED" ||
    capturedAmount?.Amount != payment.Amount ||
    capturedAmount.CurrencyCode != payment.CurrencyCode)
{
    throw new PaymentOrderException("La captura de PayPal no coincide con el monto de la orden.");
}

await orderStore.CompleteAsync(payment, capture.CaptureId,
    capturedAmount.Amount, capturedAmount.CurrencyCode, ct);
```

# 6. Estructura del Proyecto

```text
airport-app/
├── src/
│   ├── Hosts/
│   │   ├── Airport.Api/            # API REST (minimal APIs, composición de módulos)
│   │   └── Airport.Web/            # Cliente Blazor WebAssembly
│   ├── Features/
│   │   ├── Flights/                # Catálogo y búsqueda de vuelos
│   │   ├── Auth/                   # Identidad, Google OAuth y MFA
│   │   ├── Bookings/               # Reservas, boletos y comprobantes
│   │   ├── Payments/               # Integración con PayPal
│   │   └── Administration/         # Panel administrativo
│   └── BuildingBlocks/
│       ├── Airport.SharedKernel/   # Tipos compartidos
│       └── Airport.Caching/        # Caché en memoria de la aplicación
├── tests/
│   ├── Airport.UnitTests/          # Pruebas unitarias
│   └── Airport.ArchitectureTests/  # Pruebas de arquitectura
├── docs/
│   └── ARQUITECTURE.md             # Diagramas y documentación de arquitectura
├── .local/
│   ├── run.txt                     # Notas de ejecución y restauración de BD
│   └── aeropuerto-db.sql           # Respaldo para restaurar la base de datos
└── README.md
```

Cada *feature* sigue el patrón hexagonal: `Application` (casos de uso y puertos), `Domain` (entidades), `Infrastructure` (adaptadores) y `Presentation` (endpoints y páginas).

# 7. Requisitos Previos

- Sistema operativo compatible (probado en Linux).
- SDK de **.NET 10.0** (runtime incluido).
- **PostgreSQL 18** corriendo en `localhost:5432`.
- Cuenta de desarrollador de **PayPal** (modo Sandbox) con una app REST.
- Proyecto de **Google Cloud** con credenciales OAuth 2.0.
- Cuenta SMTP (usada Gmail con contraseña de aplicación) para envío de correos.

# 8. Instalación

```bash
# Clonar repositorio
git clone git@github.com:jeffersonmejia/airport-app.git
cd airport-app

# Restaurar dependencias
dotnet restore

# Restaurar la base de datos (PostgreSQL 18 local)
# Coloca aeropuerto-db.sql en .local/ y ejecuta (ver .local/run.txt):
PGOPTIONS='-c work_mem=4MB' psql -U postgres -d postgres \
  -c 'CREATE DATABASE aereopuerto_db;' \
  -c '\connect aereopuerto_db' -f .local/aeropuerto-db.sql

# Configurar secretos (ver sección 9)
dotnet user-secrets init --project src/Hosts/Airport.Api/Airport.Api.csproj
```

# 9. Configuración

La configuración pública se encuentra en `appsettings.json` y `appsettings.Development.json` de cada host (puertos, `Cors:AllowedOrigins`, `PayPal:ReturnUrl`/`CancelUrl`). Los valores sensibles se guardan en **user-secrets** del proyecto `Airport.Api` y **no** se suben al repositorio:

```bash
dotnet user-secrets set --project src/Hosts/Airport.Api/Airport.Api.csproj "ConnectionStrings:AirportDb" "Host=localhost;Port=5432;Database=aereopuerto_db;Username=postgres;Password=..."
dotnet user-secrets set --project src/Hosts/Airport.Api/Airport.Api.csproj "PayPal:ClientId" "..."
dotnet user-secrets set --project src/Hosts/Airport.Api/Airport.Api.csproj "PayPal:ClientSecret" "..."
```

# 10. Variables de Entorno

| Variable | Descripción | Obligatoria |
| -------- | ----------- | ----------- |
| `ConnectionStrings:AirportDb` | Cadena de conexión a PostgreSQL | Sí |
| `PayPal:ClientId` | Client ID de la app REST de PayPal Sandbox | Sí |
| `PayPal:ClientSecret` | Client Secret de la app REST de PayPal Sandbox | Sí |
| `Authentication:Google:ClientId` | Client ID de Google OAuth | No (deshabilita Google) |
| `Authentication:Google:ClientSecret` | Client Secret de Google OAuth | No |
| `Authentication:Google:WebCallbackUrl` | URL de retorno del callback de Google | No |
| `EmailSettings:SmtpServer` | Servidor SMTP para correos | No |
| `EmailSettings:SmtpPort` | Puerto SMTP | No |
| `EmailSettings:SenderEmail` | Remitente de los correos | No |
| `EmailSettings:Password` | Contraseña de aplicación SMTP | No |
| `Auth:Jwt:SigningKey` | Clave de firma de los JWT | Sí |

No incluir credenciales reales dentro del repositorio.

# 11. Base de Datos

## Motor de Base de Datos

- **PostgreSQL 18**, base de datos `aereopuerto_db`.

La base se restaura a partir del respaldo `.local/aeropuerto-db.sql` (ver `.local/run.txt`). Contiene dos esquemas:

- `airportdb`: datos de vuelos y aeropuertos del dominio original.
- `airport_app`: entidades de la aplicación (usuarios/identidad, órdenes, detalles, pagos y boletos).

## Diagrama Entidad–Relación (ER)

Las entidades principales de `airport_app` son:

- `identity_users` (y tablas de Identity): cuentas de usuario, roles y claims.
- `orders`: reservas realizadas (usuario, vuelo, tarifa, total, estado).
- `order_details`: datos del pasajero de cada orden.
- `payments`: intentos de pago con PayPal (estado, captura, idempotencia).
- `purchased_tickets`: boletos emitidos tras la captura del pago.

> **Figura 2.** Relaciones entre órdenes, pagos y boletos (ver `docs/ARQUITECTURE.md`).

## Scripts de Inicialización

```text
.local/
├── run.txt                  # Pasos para restaurar la base de datos
└── aeropuerto-db.sql        # Respaldo SQL completo (esquemas + datos)
```

- **aeropuerto-db.sql:** crea la base con el esquema `airportdb` (vuelos) y `airport_app` (aplicación) y sus datos iniciales.

# 12. Ejecución

```bash
# Backend (API) en http://localhost:5038
dotnet run --project src/Hosts/Airport.Api/Airport.Api.csproj --launch-profile http

# Frontend (Web) en http://localhost:5235
dotnet run --project src/Hosts/Airport.Web/Airport.Web.csproj --launch-profile http

# Con recarga automática (recomendado en desarrollo)
dotnet watch run --project src/Hosts/Airport.Api/Airport.Api.csproj --launch-profile http
dotnet watch run --project src/Hosts/Airport.Web/Airport.Web.csproj --launch-profile http
```

Puertos por defecto: API `http://localhost:5038` (https `7185`) y Web `http://localhost:5235` (https `7194`). Abre `http://localhost:5235` en el navegador.

# 13. API

| Método | Endpoint | Descripción |
| ------ | -------- | ----------- |
| GET | `/api/flights/flight` | Búsqueda de vuelos disponibles |
| GET | `/api/flights/airports` | Lista de aeropuertos |
| GET | `/api/flights/filter-options` | Opciones de filtrado (origen, destino, tarifas) |
| POST | `/api/auth/login` | Inicio de sesión (usuario interno) |
| POST | `/api/auth/account/register` | Registro de cuenta local |
| GET | `/api/auth/account/confirm-email` | Confirmación de correo |
| POST | `/api/auth/account/login` | Inicio de sesión con cuenta de correo |
| GET | `/api/auth/providers` | Disponibilidad de proveedores externos (Google) |
| GET | `/api/auth/google/login` | Redirección al inicio de sesión de Google |
| GET | `/api/auth/google/callback` | Callback de Google OAuth |
| GET | `/api/auth/mfa/setup` | Configuración de MFA (QR) |
| POST | `/api/auth/mfa/enable` | Activar MFA |
| POST | `/api/auth/mfa/disable` | Desactivar MFA |
| POST | `/api/auth/mfa/sign-in` | Verificación de código MFA al iniciar sesión |
| GET | `/api/auth/session` | Sesión actual (cookies) |
| POST | `/api/auth/logout` | Cerrar sesión |
| POST | `/api/bookings/orders` | Crear una reserva de boleto |
| GET | `/api/bookings/orders/{orderId}` | Detalle de una reserva |
| GET | `/api/bookings/history` | Historial de reservas del cliente |
| GET | `/api/bookings/orders/{orderId}/receipt` | Comprobante de una compra |
| POST | `/api/payments/paypal/orders` | Crear orden de pago en PayPal |
| POST | `/api/payments/paypal/orders/{orderId}/capture` | Capturar el pago aprobado |
| GET | `/api/admin/database-summary` | Resumen de la base de datos (admin) |
| GET | `/api/admin/commerce` | Resumen del comercio (admin) |

# 14. Pruebas

- Pruebas unitarias de casos de uso de las *features* (`Airport.UnitTests`).
- Pruebas de arquitectura que validan las reglas de dependencia hexagonal (`Airport.ArchitectureTests`).

Comando para ejecutar las pruebas:

```bash
dotnet test tests/Airport.UnitTests/Airport.UnitTests.csproj --nologo --verbosity quiet -p:WarningLevel=0 --tl:off
```

# 15. Seguridad

- **Autenticación** con ASP.NET Core Identity: cuentas locales con confirmación de correo, sesión por cookies en el Web y **JWT** para la API.
- **Autorización** por roles (`Client`, `Admin`) y políticas en los endpoints.
- **Autenticación multifactor (MFA/TOTP)** configurable desde la cuenta del usuario.
- **Google OAuth 2.0** para inicio de sesión y registro externo.
- **Gestión de credenciales** mediante `dotnet user-secrets`; no se almacenan secretos en el repositorio.
- **Validación de entradas** con `FluentValidation` en los comandos de aplicación.
- **Pagos** con PayPal Sandbox e idempotencia (`PayPal-Request-Id`) para evitar cobros duplicados.
- **Protección de información sensible**: no se registran ni exponen claves, contraseñas ni secretos.

# 16. Rendimiento

- Consultas de catálogo con `AsNoTracking()` y filtrado en base de datos.
- **Caché en memoria** (`Airport.Caching`) para datos de lectura frecuente (aeropuertos, filtros).
- Transacciones aisladas (`Serializable`) para la captura de pagos, garantizando consistencia entre orden, pago y boleto.
- Índices únicos en claves de negocio críticas (`idempotency_key`, `provider_order_id`, `provider_capture_id`, `order_id` de boletos).
- Aplicación ligera orientada a un equipo de desarrollo; la escalabilidad horizontal se logra escalando los *hosts* Web y API por separado.

# 17. Versionado

Para mantener un historial de cambios organizado en Git y GitHub, el proyecto utiliza la siguiente convención de commits:

| Prefijo     | Descripción                                                  |
| ----------- | ------------------------------------------------------------ |
| `feat:`     | Incorporación de nuevas funcionalidades.                     |
| `docs:`     | Creación o actualización de documentación.                   |
| `fix:`      | Corrección de errores.                                       |
| `refactor:` | Reestructuración del código sin modificar su comportamiento. |

**Ejemplos**

```text
feat: implementar autenticación JWT

docs: actualizar README

fix: corregir validación del formulario

refactor: reorganizar capa de servicios
```

# 18. Autores

**Jefferson Mejía**

# 19. Contacto

**Jefferson Mejía**

Correo electrónico: **[jeffersonmejiach01@gmail.com](mailto:jeffersonmejiach01@gmail.com)**
