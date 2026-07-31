# 003. Organización del proyecto, Identity y seguridad

> Este archivo forma parte de la división de `requirements.md`.

---

## 1. Organización obligatoria del proyecto

### 1.1 Arquitectura y estructura

El proyecto usa **arquitectura hexagonal** (Domain, Application, Infrastructure y
Presentation con puertos y adaptadores), **vertical slices** (cada caso de uso es un
slice completo dentro de `Application`) y **screaming architecture** (las carpetas se
nombran por concepto del negocio), tal como está organizado el proyecto actual.

- [ ] Organización screaming: `src/Features/<Concepto>` (Flights, Bookings, Auth, Administration, ...).
- [ ] Cada feature con las fronteras hexagonales: `Domain`, `Application`, `Infrastructure` y `Presentation` (Api + Web).
- [ ] Hosts delgados: `src/Hosts/Airport.Api` y `src/Hosts/Airport.Web`.
- [ ] Building blocks transversales en `src/BuildingBlocks` (SharedKernel, Caching).
- [ ] `Migrations` dentro de `Infrastructure`.
- [ ] `wwwroot`, CSS y componentes del negocio dentro de `Presentation/Web`.
- [ ] Cada caso de uso vive en `Application` como un vertical slice (`GetFlight`, `SearchFlights`, `CreateBooking`, `Login`, ...).

### 1.2 Separación de lógica

La solución separa responsabilidades por frontera hexagonal:

- [ ] Acceso a datos y EF Core → `Infrastructure`.
- [ ] Reglas del proceso de compra → `Domain` y `Application`.
- [ ] Gestión de pagos → slice en `Application` + adaptador de pasarela en `Infrastructure`.
- [ ] Autenticación → feature `Auth` independiente.
- [ ] Presentación de información → `Presentation/Api` y `Presentation/Web`.
- [ ] Validación de formularios y casos de uso → validadores del slice en `Application`.

> [!CAUTION]
> No debe colocarse toda la lógica dentro de una sola acción del controlador, endpoint o página.

### 1.3 Separación mínima recomendada para evitar penalizaciones

- [ ] Slice `SearchFlights` y `GetFlight` en `Features/Flights/Application`.
- [ ] Puerto `IFlightReader` en `Application/Ports`.
- [ ] Adaptador `PostgresFlightReader` en `Features/Flights/Infrastructure`.
- [ ] Slices de creación de órdenes, tarifas e historial en `Features/Bookings/Application`.
- [ ] Reglas de cálculo de tarifas en `Features/Bookings/Domain`.
- [ ] Slice de pago con puerto de pasarela (`IPaymentGateway`) y adaptador en `Infrastructure`.
- [ ] Slices `Login`, `Logout` y `GetCurrentUser` en `Features/Auth/Application`.
- [ ] Módulo administrativo en `Features/Administration`.
- [ ] Contratos `Response` por slice para búsqueda, paginación, detalle, pago y comprobante (en lugar de ViewModels globales).

---

## 2. ASP.NET Core Identity

### 2.1 Funciones mínimas

- [ ] Registro de usuarios.
- [ ] Inicio de sesión.
- [ ] Cierre de sesión.
- [ ] Persistencia mediante cookies.
- [ ] Protección con `[Authorize]`.
- [ ] Roles.
- [ ] Página de acceso denegado.
- [ ] Asociación de cada orden con el usuario autenticado.
- [ ] Historial individual de operaciones.

### 2.2 Roles mínimos

- [ ] `Administrador`
- [ ] `Cliente`

### 2.3 Permisos del Administrador

El administrador debe poder:

- [ ] Consultar todas las órdenes.
- [ ] Consultar todos los pagos.
- [ ] Revisar transacciones.
- [ ] Administrar el proceso de compra de boletos.
- [ ] Consultar reportes generales.
- [ ] Revisar operaciones de los clientes.

### 2.4 Permisos del Cliente

El cliente debe poder:

- [ ] Consultar información aeroportuaria.
- [ ] Buscar vuelos.
- [ ] Crear órdenes.
- [ ] Realizar pagos.
- [ ] Consultar sus propias operaciones.
- [ ] Acceder únicamente a sus registros.

### 2.5 Restricciones

- [ ] No simular el inicio de sesión con variables.
- [ ] No simular el usuario mediante parámetros de URL.
- [ ] No escribir nombres de usuarios directamente en el código.
- [ ] No permitir que un cliente consulte órdenes de otro usuario.
- [ ] No permitir acceso administrativo sin el rol correspondiente.

---

## 3. Seguridad

### 3.1 Almacenamiento de credenciales

Las credenciales deben almacenarse mediante:

- [ ] Variables de entorno.
- [ ] Secret Manager.
- [ ] Archivos de configuración excluidos del repositorio.

### 3.2 Información prohibida en GitHub

No publicar:

- [ ] `ClientSecret`
- [ ] `AccessToken`
- [ ] `StoreID` privado
- [ ] Contraseñas
- [ ] Cadenas de conexión reales
- [ ] Tokens de sesión
- [ ] Claves de API

### 3.3 Archivo de ejemplo

- [ ] Incluir `appsettings.Example.json`.
- [ ] Usar valores ficticios o marcadores.
- [ ] Explicar cómo completar la configuración local.
- [ ] Confirmar que `.gitignore` excluya los archivos sensibles.

### 3.4 Controles adicionales de alta prioridad

- [ ] Usar protección antiforgery en formularios POST.
- [ ] Usar validación de modelo con `ModelState`.
- [ ] Verificar propiedad de la orden en el backend.
- [ ] Aplicar autorización por rol en controladores administrativos.
- [ ] No registrar secretos en logs.
- [ ] No devolver detalles internos de excepciones al usuario.
- [ ] Validar callbacks o respuestas de la pasarela.
