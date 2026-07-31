# 005 — Autenticación, roles y dashboard

## Objetivo

Implementar el acceso de empleados mediante la feature `Auth`, traducir los
departamentos existentes en `airportdb.employee` a roles de autorización y ofrecer
un Home específico por rol. El rol `Admin` dispondrá además de un resumen global de
la base con nombres funcionales, sin exponer nombres internos de tablas.

Esta etapa aprovechará el esquema restaurado y no agregará tablas de usuarios o
roles. Los pasajeros continuarán siendo entidades del negocio asociadas a reservas;
la consulta y compra de vuelos no requerirá una cuenta de pasajero en este alcance.

## Roles

La base no contiene una tabla de roles. La identidad del empleado se obtendrá desde
`airportdb.employee` y su enum `department` se transformará en roles de la aplicación
al iniciar sesión.

| Departamento en PostgreSQL | Rol de aplicación | Alcance inicial |
|---|---|---|
| `Marketing` | `Marketing` | Información y operaciones de marketing |
| `Buchhaltung` | `Accounting` | Información contable y financiera |
| `Management` | `Management`, `Admin` | Administración y acceso global |
| `Logistik` | `Logistics` | Información y operaciones logísticas |
| `Flugfeld` | `AirfieldOperations` | Operaciones de pista y vuelos |

`Admin` será un rol de autorización derivado: todo empleado de `Management` recibirá
los claims `Management` y `Admin`. Esta decisión evita modificar la base durante el
examen y deberá revisarse si posteriormente no todos los miembros de Management
deben tener privilegios administrativos.

Los roles se emitirán como claims del JWT desde `Auth`; ninguna feature de negocio
leerá contraseñas ni interpretará directamente el campo `department`. Los endpoints
protegidos aplicarán políticas declarativas y el acceso administrativo exigirá el
rol `Admin`.

## Pasajeros y visitantes

- Un visitante podrá consultar vuelos sin autenticarse.
- Al comprar o reservar se utilizarán `passenger`, `passengerdetails` y `booking`.
- `Passenger` no será un rol de autenticación en esta etapa porque esas tablas no
  contienen credenciales.
- El registro, inicio de sesión y recuperación de cuentas de pasajeros quedan fuera
  de alcance. Si se incorporan después, Auth deberá usar una cuenta común vinculable
  a un empleado o pasajero, sin añadir contraseñas directamente a `passenger`.

## Home por rol

Después del login, la navegación mostrará un Home acorde con los claims de la
identidad. Cada rol verá accesos y datos de su área; `Admin` verá el resumen global y
podrá acceder a todos los módulos habilitados.

El Home de `Admin` incluirá:

- Total global de registros considerados por el dashboard.
- Gráfico circular con la distribución por categorías funcionales.
- Leyenda, cantidades y porcentajes accesibles además del color.
- Fecha y hora de la última actualización del resumen.
- Estados de carga, vacío, error y datos desactualizados.

La interfaz no mostrará nombres físicos de tablas. Se aplicarán, como mínimo, estas
etiquetas:

| Tabla | Etiqueta visible |
|---|---|
| `flight` | Vuelos |
| `booking` | Reservas |
| `passenger` | Pasajeros |
| `passengerdetails` | Detalles de pasajeros |
| `employee` | Empleados |
| `airplane` | Aeronaves |
| `airplane_type` | Tipos de aeronave |
| `airline` | Aerolíneas |
| `airport` | Aeropuertos |
| `airport_geo` | Ubicaciones de aeropuertos |
| `airport_reachable` | Rutas entre aeropuertos |
| `flightschedule` | Programación de vuelos |
| `flight_log` | Historial de vuelos |
| `weatherdata` | Registros meteorológicos |

## Conteos y rendimiento

La base contiene decenas de millones de filas. El Home no ejecutará un `COUNT(*)`
de todas las tablas en cada solicitud ni transferirá registros para contarlos en la
aplicación.

- Infrastructure realizará los conteos exclusivamente en PostgreSQL.
- El resumen se calculará fuera de la carga interactiva o se reutilizará desde
  caché con una vigencia explícita.
- La primera versión podrá usar estimaciones del catálogo para una respuesta rápida,
  identificándolas como aproximadas en la interfaz.
- Si se requieren cifras exactas, se actualizarán mediante un proceso controlado y
  se servirá el último resultado disponible.
- El endpoint devolverá sólo etiqueta funcional, cantidad, porcentaje, total y fecha
  de actualización.

El gráfico podrá agrupar categorías pequeñas cuando las catorce porciones dificulten
la lectura, pero el total deberá corresponder siempre a la suma de las cantidades
mostradas o agrupadas.

## Slices previstos

Dentro de `Auth`:

- `Login`: validar al empleado y emitir los roles correspondientes.
- `Logout`: invalidar la sesión activa.
- `GetCurrentUser`: devolver identidad, roles y permisos visibles.

Dentro de una feature administrativa independiente, creada sólo al implementar el
dashboard:

- `GetDatabaseSummary`: obtener el total y la distribución cacheada para `Admin`.

La UI del dashboard pertenecerá a la presentación web de esa feature. `Auth` será
responsable únicamente de identidad y autorización, no de consultar estadísticas.

## Implementación por etapas

- [ ] Mapear de forma explícita los cinco valores de `employee.department`.
- [ ] Implementar `Login` sin comparar ni almacenar contraseñas en texto plano.
- [ ] Emitir claims de rol y añadir `Admin` cuando el departamento sea `Management`.
- [ ] Implementar `Logout` y `GetCurrentUser`.
- [ ] Definir y probar políticas de autorización por rol.
- [ ] Adaptar navegación y Home a los claims del usuario.
- [ ] Crear `GetDatabaseSummary` protegido exclusivamente para `Admin`.
- [ ] Implementar conteos eficientes, caché y fecha de actualización.
- [ ] Presentar total, gráfico, leyenda y etiquetas funcionales accesibles.
- [ ] Probar que los demás roles no accedan al resumen administrativo.
- [ ] Documentar el criterio usado si los totales son aproximados.

## Criterios de aceptación

- Un empleado autenticado recibe los roles derivados de su departamento.
- Un empleado de `Management` recibe también `Admin` y accede al Home administrativo.
- Un usuario sin `Admin` obtiene una respuesta prohibida al solicitar el resumen.
- Ninguna respuesta expone contraseñas, nombres físicos de tablas ni detalles de
  conexión.
- El Home no bloquea la interfaz realizando conteos completos de decenas de millones
  de filas en cada carga.
- El total y la distribución son coherentes y comunican si los valores son exactos o
  aproximados.
