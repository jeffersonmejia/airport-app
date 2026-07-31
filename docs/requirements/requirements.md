# Requerimientos estrictos — Examen Airport, Tipo 1: Compra de boletos

> Lista maestra de implementación, pruebas, documentación, repositorio y sustentación.
>
> **Propósito:** controlar el cumplimiento total del examen y reducir omisiones que puedan afectar la calificación.
>
> **Regla de uso:** un requisito solo se marca como cumplido cuando existe implementación funcional, evidencia verificable y documentación correspondiente.

---

## 1. Identificación del examen

| Campo | Valor |
|---|---|
| Estudiante | Jefferson Paul Mejía Chávez |
| Modalidad | Individual |
| Tipo asignado | **Tipo 1** |
| Ejercicio | **Compra de boletos** |
| Base de datos | **Airport de Postgres Pro** |
| Pasarela | PayPal Sandbox o PayPhone en ambiente de pruebas |
| Apertura | Viernes 31 de julio de 2026, 07:00 |
| Entrega | Domingo 2 de agosto de 2026, 17:00 |
| Puntaje | 20 puntos |
| Entrega en plataforma | Un único archivo PDF |
| Tamaño máximo | 20 MB |
| Repositorio | GitHub público |

### Datos todavía por completar

- [ ] Pasarela seleccionada: `________________________`
- [ ] Fecha de realización: `________________________`
- [ ] URL pública de GitHub: `________________________`
- [ ] URL o referencia del video de sustentación, cuando corresponda: `________________________`

> [!CAUTION]
> No se permite cambiar el ejercicio asignado.

---

# 2. Resultado final obligatorio

La aplicación debe ser un sistema web funcional de compra de boletos aeroportuarios que permita:

- [ ] Registrar usuarios.
- [ ] Iniciar sesión.
- [ ] Cerrar sesión.
- [ ] Aplicar autenticación persistente mediante cookies.
- [ ] Diferenciar los roles `Administrador` y `Cliente`.
- [ ] Proteger rutas y operaciones con autorización.
- [ ] Consultar datos reales de la base Airport.
- [ ] Seleccionar un aeropuerto de origen.
- [ ] Seleccionar un aeropuerto de destino.
- [ ] Elegir una fecha de viaje.
- [ ] Buscar vuelos disponibles.
- [ ] Aplicar búsqueda, filtros y ordenamiento.
- [ ] Paginar físicamente los resultados desde PostgreSQL.
- [ ] Consultar el detalle de un vuelo.
- [ ] Seleccionar una tarifa.
- [ ] Crear una orden pendiente.
- [ ] Registrar el detalle de la orden.
- [ ] Procesar el pago mediante PayPal Sandbox o PayPhone de pruebas.
- [ ] Verificar el resultado del pago en el backend.
- [ ] Registrar el boleto adquirido.
- [ ] Registrar el pago y la transacción en PostgreSQL.
- [ ] Impedir transacciones duplicadas.
- [ ] Mostrar un comprobante.
- [ ] Mostrar el historial individual del cliente.
- [ ] Permitir al administrador revisar órdenes, pagos y transacciones.

---

# 3. Tecnologías obligatorias

## 3.1 Stack mínimo

- [ ] ASP.NET Core MVC.
- [ ] Entity Framework Core.
- [ ] PostgreSQL.
- [ ] Base de datos Airport.
- [ ] Proveedor Npgsql.
- [ ] ASP.NET Core Identity.
- [ ] Cookies de autenticación.
- [ ] Roles y autorización.
- [ ] Consultas LINQ.
- [ ] Paginación física mediante `Skip()` y `Take()`.
- [ ] Bootstrap.
- [ ] GitHub.
- [ ] PayPal Sandbox o PayPhone en pruebas.

## 3.2 Capacidades técnicas que deben demostrarse

- [ ] Arquitectura MVC.
- [ ] Acceso a datos mediante Entity Framework Core.
- [ ] Uso de PostgreSQL.
- [ ] Database First para Airport.
- [ ] Migraciones para las tablas propias.
- [ ] ASP.NET Core Identity.
- [ ] Protección de rutas con `[Authorize]`.
- [ ] Protección por rol.
- [ ] Validación del lado del servidor.
- [ ] Manejo de errores.
- [ ] Creación y seguimiento de órdenes.
- [ ] Integración y verificación real del pago en Sandbox.
- [ ] Buenas prácticas de seguridad.

---

# 4. Base de datos Airport

## 4.1 Instalación obligatoria

- [ ] Descargar la base demostrativa Airport desde Postgres Pro.
- [ ] Instalar, como mínimo, la versión correspondiente a tres meses.
- [ ] Utilizar como referencia mínima `demo-20250901-3m.sql.gz`.
- [ ] Descomprimir el archivo `.gz`.
- [ ] Obtener el script `.sql`.
- [ ] Importar el script mediante `psql`.
- [ ] No abrir el archivo completo con Query Tool de pgAdmin.
- [ ] Usar `ON_ERROR_STOP=1`.
- [ ] Actualizar el listado de bases de datos en pgAdmin.
- [ ] Confirmar la creación de la base.
- [ ] Confirmar la existencia del esquema `bookings`.
- [ ] Confirmar que la tabla `bookings.flights` contiene datos.

### Comando de referencia

```powershell
& "C:\Program Files\PostgreSQL\17\bin\psql.exe" `
-h localhost `
-p 5432 `
-U postgres `
-d postgres `
-v ON_ERROR_STOP=1 `
-f "C:\BD\airport\demo-20250901-3m.sql"
```

### Verificación mínima

```sql
SELECT current_database();

SELECT COUNT(*)
FROM bookings.flights;
```

## 4.2 Restricciones de uso

- [ ] Utilizar datos reales de Airport.
- [ ] Conservar la estructura original de Airport.
- [ ] No alterar destructivamente las tablas originales.
- [ ] Crear tablas propias para las operaciones de la aplicación.
- [ ] Utilizar únicamente las entidades de Airport necesarias para el Tipo 1.

---

# 5. Entidades requeridas para el Tipo 1

## 5.1 Tablas de Airport sugeridas

- [ ] `airports_data`
- [ ] `flights`
- [ ] `routes`
- [ ] `bookings`
- [ ] `tickets`
- [ ] `ticket_flights`

> Las tablas anteriores son sugeridas por el enunciado. Deben seleccionarse las necesarias para implementar correctamente el flujo de compra.

## 5.2 Tablas propias sugeridas

- [ ] `Orders`
- [ ] `OrderDetails`
- [ ] `PurchasedTickets`
- [ ] `Payments`
- [ ] `TransactionHistory`

## 5.3 Tablas adicionales mínimas equivalentes

El proyecto debe crear tablas equivalentes a:

- [ ] `Orders`
- [ ] `OrderDetails`
- [ ] `Payments`
- [ ] `TransactionHistory`

## 5.4 Campos mínimos de `Payments`

La tabla de pagos debe contener, como mínimo:

- [ ] `Id`
- [ ] `OrderId`
- [ ] `UserId`
- [ ] `Gateway`
- [ ] `ExternalTransactionId`
- [ ] `Amount`
- [ ] `Currency`
- [ ] `Status`
- [ ] `CreationDate`
- [ ] `ConfirmationDate`
- [ ] `ResponseMessage`

### Restricción crítica

- [ ] `ExternalTransactionId` debe utilizarse para impedir el registro duplicado de una misma transacción.
- [ ] Debe existir una restricción única o una validación equivalente que garantice la idempotencia.
- [ ] La prevención de duplicados debe demostrarse mediante una prueba.

---

# 6. Entity Framework Core

## 6.1 Conexión y proveedor

- [ ] Instalar y configurar `Npgsql.EntityFrameworkCore.PostgreSQL`.
- [ ] Configurar la cadena de conexión sin publicarla en GitHub.
- [ ] Conectar la aplicación con Airport.
- [ ] Ejecutar una consulta real desde Entity Framework Core.

## 6.2 Database First

- [ ] Generar el contexto de Airport.
- [ ] Generar únicamente los modelos necesarios para Compra de boletos.
- [ ] Revisar nombres, tipos, claves y relaciones generadas.
- [ ] Verificar que las consultas se ejecuten contra el esquema `bookings`.

Ejemplo de referencia:

```powershell
dotnet ef dbcontext scaffold `
"Host=localhost;Port=5432;Database=demo;Username=postgres;Password=CLAVE" `
Npgsql.EntityFrameworkCore.PostgreSQL `
--context AirportContext `
--output-dir Models/Airport
```

## 6.3 Migraciones

- [ ] Crear las tablas propias mediante migraciones.
- [ ] Incluir las migraciones en el repositorio.
- [ ] Aplicar las migraciones a PostgreSQL.
- [ ] Documentar el proceso en el informe.
- [ ] Confirmar que una instalación limpia pueda ejecutar las migraciones.

```powershell
dotnet ef migrations add InitialApplicationTables
dotnet ef database update
```

## 6.4 Contextos

Se permite:

- [ ] Un contexto para Airport.
- [ ] Otro contexto para Identity y tablas propias.

Debe documentarse:

- [ ] Qué contexto administra cada conjunto de entidades.
- [ ] Cómo se configuran ambos contextos.
- [ ] Cómo se evita mezclar responsabilidades.

---

# 7. Organización obligatoria del proyecto

## 7.1 Carpetas mínimas

- [ ] `Controllers`
- [ ] `Models`
- [ ] `Views`
- [ ] `Data`
- [ ] `Services`
- [ ] `ViewModels`
- [ ] `Migrations`
- [ ] `wwwroot`

## 7.2 Separación de lógica

La solución debe separar:

- [ ] Acceso a datos.
- [ ] Reglas del proceso de compra.
- [ ] Gestión de pagos.
- [ ] Autenticación.
- [ ] Presentación de información.
- [ ] Validación de formularios.

> [!CAUTION]
> No debe colocarse toda la lógica dentro de una sola acción del controlador.

## 7.3 Separación mínima recomendada para evitar penalizaciones

- [ ] Controlador de búsqueda y detalle de vuelos.
- [ ] Servicio de consulta de vuelos.
- [ ] Servicio de cálculo de tarifas.
- [ ] Controlador de órdenes.
- [ ] Servicio de órdenes.
- [ ] Servicio de pagos.
- [ ] Controlador de historial.
- [ ] Controlador administrativo.
- [ ] ViewModels específicos para búsqueda, paginación, detalle, pago y comprobante.

---

# 8. ASP.NET Core Identity

## 8.1 Funciones mínimas

- [ ] Registro de usuarios.
- [ ] Inicio de sesión.
- [ ] Cierre de sesión.
- [ ] Persistencia mediante cookies.
- [ ] Protección con `[Authorize]`.
- [ ] Roles.
- [ ] Página de acceso denegado.
- [ ] Asociación de cada orden con el usuario autenticado.
- [ ] Historial individual de operaciones.

## 8.2 Roles mínimos

- [ ] `Administrador`
- [ ] `Cliente`

## 8.3 Permisos del Administrador

El administrador debe poder:

- [ ] Consultar todas las órdenes.
- [ ] Consultar todos los pagos.
- [ ] Revisar transacciones.
- [ ] Administrar el proceso de compra de boletos.
- [ ] Consultar reportes generales.
- [ ] Revisar operaciones de los clientes.

## 8.4 Permisos del Cliente

El cliente debe poder:

- [ ] Consultar información aeroportuaria.
- [ ] Buscar vuelos.
- [ ] Crear órdenes.
- [ ] Realizar pagos.
- [ ] Consultar sus propias operaciones.
- [ ] Acceder únicamente a sus registros.

## 8.5 Restricciones

- [ ] No simular el inicio de sesión con variables.
- [ ] No simular el usuario mediante parámetros de URL.
- [ ] No escribir nombres de usuarios directamente en el código.
- [ ] No permitir que un cliente consulte órdenes de otro usuario.
- [ ] No permitir acceso administrativo sin el rol correspondiente.

---

# 9. Requerimientos funcionales específicos del Tipo 1

## RF-01. Selección de origen

- [ ] Mostrar aeropuertos reales de Airport.
- [ ] Permitir seleccionar un aeropuerto de origen.
- [ ] Validar que el origen exista.
- [ ] No confiar únicamente en el valor recibido desde el navegador.

**Criterio de aceptación:** el servidor valida el código o identificador contra PostgreSQL.

## RF-02. Selección de destino

- [ ] Mostrar aeropuertos reales de Airport.
- [ ] Permitir seleccionar un aeropuerto de destino.
- [ ] Validar que el destino exista.
- [ ] Impedir que origen y destino sean iguales.

**Criterio de aceptación:** no se procesa una búsqueda inválida.

## RF-03. Fecha

- [ ] Permitir elegir una fecha.
- [ ] Validar el formato.
- [ ] Consultar vuelos para la fecha seleccionada.
- [ ] Incluir una consulta LINQ por fecha.

## RF-04. Búsqueda de vuelos

- [ ] Consultar vuelos reales.
- [ ] Aplicar origen.
- [ ] Aplicar destino.
- [ ] Aplicar fecha.
- [ ] Aplicar los filtros adicionales requeridos.
- [ ] No cargar todos los vuelos en memoria.
- [ ] Ejecutar la consulta paginada en PostgreSQL.

## RF-05. Filtros

- [ ] Incluir una búsqueda por texto.
- [ ] Incluir al menos dos filtros.
- [ ] Conservar los filtros al cambiar de página.
- [ ] Conservar los filtros al cambiar el ordenamiento.

Ejemplos válidos para este ejercicio:

- Texto: número de vuelo, nombre de aeropuerto o ciudad.
- Filtro 1: estado del vuelo.
- Filtro 2: aeropuerto de origen, destino, fecha o aeronave.
- Filtro adicional: franja horaria o disponibilidad.

## RF-06. Ordenamiento

- [ ] Incorporar al menos un ordenamiento.
- [ ] Aplicarlo antes de `Skip()` y `Take()`.
- [ ] Conservarlo durante la navegación de páginas.

Ejemplos:

- Fecha y hora de salida.
- Duración.
- Número de vuelo.
- Tarifa calculada.

## RF-07. Detalle del vuelo

- [ ] Mostrar la información principal del vuelo.
- [ ] Mostrar origen.
- [ ] Mostrar destino.
- [ ] Mostrar fecha y hora programadas.
- [ ] Mostrar estado.
- [ ] Mostrar aeronave o ruta, cuando corresponda.
- [ ] Validar nuevamente la existencia del vuelo en el servidor.

## RF-08. Selección de tarifa

- [ ] Permitir seleccionar una tarifa.
- [ ] Definir las tarifas disponibles en el servidor o base de datos.
- [ ] Calcular el monto en el servidor.
- [ ] No aceptar como confiable un precio enviado desde el navegador.
- [ ] Registrar la tarifa elegida en `OrderDetails`.

## RF-09. Creación de orden

- [ ] Exigir que el usuario esté autenticado.
- [ ] Consultar nuevamente el vuelo seleccionado.
- [ ] Calcular nuevamente el monto.
- [ ] Crear la orden con estado `Pendiente`.
- [ ] Asociar la orden con el usuario autenticado.
- [ ] Registrar el detalle.
- [ ] Guardar fechas de creación y estado.

## RF-10. Pago

- [ ] Crear una solicitud real en Sandbox.
- [ ] Redirigir o integrar el flujo oficial de la pasarela.
- [ ] Verificar el resultado desde el backend.
- [ ] No considerar aprobado un pago basándose únicamente en parámetros del navegador.
- [ ] Registrar el identificador externo.
- [ ] Actualizar orden y pago según el resultado.
- [ ] Manejar aprobación.
- [ ] Manejar cancelación, rechazo o fallo.

## RF-11. Registro del boleto adquirido

- [ ] Registrar el boleto después de un pago aprobado.
- [ ] Asociarlo con la orden.
- [ ] Asociarlo con el usuario.
- [ ] Asociarlo con el vuelo.
- [ ] Registrar la tarifa y el monto.
- [ ] Impedir emitirlo si el pago no está aprobado.
- [ ] Impedir emitirlo dos veces por la misma transacción.

## RF-12. Comprobante

- [ ] Mostrar número o identificador de orden.
- [ ] Mostrar datos del cliente.
- [ ] Mostrar datos del vuelo.
- [ ] Mostrar origen y destino.
- [ ] Mostrar fecha.
- [ ] Mostrar tarifa.
- [ ] Mostrar monto.
- [ ] Mostrar moneda.
- [ ] Mostrar estado.
- [ ] Mostrar pasarela.
- [ ] Mostrar identificador de transacción.
- [ ] Mostrar fecha de confirmación.

## RF-13. Historial del cliente

- [ ] Mostrar únicamente las operaciones del usuario autenticado.
- [ ] Mostrar órdenes.
- [ ] Mostrar pagos.
- [ ] Mostrar boletos adquiridos.
- [ ] Permitir consultar el detalle.
- [ ] Proteger contra modificación de identificadores en la URL.

## RF-14. Administración

- [ ] Listar todas las órdenes.
- [ ] Listar todos los pagos.
- [ ] Listar transacciones.
- [ ] Filtrar por usuario.
- [ ] Filtrar por estado.
- [ ] Consultar reportes generales.
- [ ] Proteger todo el módulo con rol `Administrador`.

---

# 10. Consultas LINQ obligatorias

La aplicación debe utilizar LINQ para recuperar, filtrar, ordenar y proyectar información.

## 10.1 Operadores que deben utilizarse según corresponda

- [ ] `Where()`
- [ ] `Select()`
- [ ] `OrderBy()`
- [ ] `OrderByDescending()`
- [ ] `GroupBy()`
- [ ] `Count()`
- [ ] `Sum()`
- [ ] `Average()`
- [ ] `Any()`
- [ ] `Include()`
- [ ] `AsNoTracking()`
- [ ] `Skip()`
- [ ] `Take()`

## 10.2 Evidencias mínimas exigidas

| Requisito LINQ | Implementación propuesta | Estado |
|---|---|---|
| Búsqueda por texto | Número de vuelo, aeropuerto o ciudad | [ ] |
| Filtro 1 | Origen o destino | [ ] |
| Filtro 2 | Estado o fecha | [ ] |
| Ordenamiento | Salida programada ascendente o descendente | [ ] |
| Consulta con relaciones | Vuelo con aeropuerto, ruta o aeronave | [ ] |
| Proyección con `Select()` | ViewModel del resultado de búsqueda | [ ] |
| Conteo o totalización | Total de vuelos o total de ventas | [ ] |
| Consulta por fecha | Vuelos de la fecha seleccionada | [ ] |
| Consulta por estado | Scheduled, Departed, Cancelled u otro estado real | [ ] |
| Consulta paginada | `Skip()` y `Take()` antes de `ToListAsync()` | [ ] |

## 10.3 Control estricto

- [ ] Las consultas se ejecutan en PostgreSQL.
- [ ] No se llama `ToListAsync()` antes de filtrar y paginar.
- [ ] Se utiliza `AsNoTracking()` en consultas de solo lectura.
- [ ] Se proyecta únicamente la información necesaria.
- [ ] El código LINQ se documenta en el informe como texto.
- [ ] La sustentación explica qué parte se traduce a SQL.

---

# 11. Paginación física obligatoria

## 11.1 Reglas

- [ ] Recuperar los registros poco a poco desde PostgreSQL.
- [ ] Contar los registros con `CountAsync()`.
- [ ] Ordenar antes de paginar.
- [ ] Aplicar `Skip()`.
- [ ] Aplicar `Take()`.
- [ ] Ejecutar `ToListAsync()` después de `Skip()` y `Take()`.
- [ ] No cargar todos los registros para paginar en memoria.

### Patrón correcto

```csharp
var consulta = _context.Flights
    .AsNoTracking()
    .Where(f => f.Status == "Scheduled");

var totalRegistros = await consulta.CountAsync();

var vuelos = await consulta
    .OrderBy(f => f.ScheduledDeparture)
    .Skip((pagina - 1) * tamanioPagina)
    .Take(tamanioPagina)
    .ToListAsync();
```

### Patrón prohibido

```csharp
var todos = await consulta.ToListAsync();

var paginaActual = todos
    .Skip((pagina - 1) * tamanioPagina)
    .Take(tamanioPagina);
```

## 11.2 Información visible en cada listado principal

- [ ] Página actual.
- [ ] Total de páginas.
- [ ] Total de registros.
- [ ] Tamaño de página.
- [ ] Botón anterior.
- [ ] Botón siguiente.
- [ ] Filtros.
- [ ] Ordenamiento.
- [ ] Conservación de filtros al cambiar de página.
- [ ] Conservación del ordenamiento al cambiar de página.
- [ ] Deshabilitar anterior en la primera página.
- [ ] Deshabilitar siguiente en la última página.

---

# 12. Flujo obligatorio de pago

Debe cumplirse exactamente el siguiente proceso:

1. [ ] El usuario inicia sesión.
2. [ ] El usuario consulta información de Airport.
3. [ ] El usuario selecciona un vuelo y una tarifa.
4. [ ] El servidor vuelve a consultar el vuelo seleccionado.
5. [ ] El servidor calcula el monto.
6. [ ] Se crea una orden con estado `Pendiente`.
7. [ ] Se registran los detalles de la orden.
8. [ ] Se crea la solicitud de pago.
9. [ ] El usuario completa el proceso en Sandbox.
10. [ ] El backend verifica el resultado.
11. [ ] Se actualiza el estado de la orden.
12. [ ] Se registra la transacción en PostgreSQL.
13. [ ] Se muestra un comprobante.
14. [ ] La operación aparece en el historial del usuario.

## 12.1 Condiciones no negociables

- [ ] La pasarela forma parte del flujo real del ejercicio.
- [ ] No basta con colocar un botón.
- [ ] No basta con mostrar una pantalla simulada.
- [ ] El monto se recalcula en el servidor.
- [ ] El backend verifica la transacción.
- [ ] El pago se registra en PostgreSQL.
- [ ] El boleto se emite únicamente tras aprobación.
- [ ] El resultado fallido o cancelado también se registra correctamente.

---

# 13. Estados mínimos

Las órdenes y pagos deben manejar, como mínimo:

- [ ] `Pendiente`
- [ ] `Aprobado`
- [ ] `Cancelado`
- [ ] `Rechazado`
- [ ] `Fallido`

## 13.1 Transiciones que deben controlarse

- [ ] Orden nueva → `Pendiente`.
- [ ] Pago verificado y aprobado → orden `Aprobado`.
- [ ] Cancelación del usuario → `Cancelado`.
- [ ] Rechazo de la pasarela → `Rechazado`.
- [ ] Error técnico o verificación fallida → `Fallido`.
- [ ] No confirmar la compra sin pago aprobado.
- [ ] No registrar dos veces la misma transacción.

---

# 14. Funcionalidades comunes obligatorias

- [ ] Página principal.
- [ ] Navegación responsive.
- [ ] Registro.
- [ ] Inicio de sesión.
- [ ] Cierre de sesión.
- [ ] Menú según rol.
- [ ] Listado paginado.
- [ ] Búsqueda.
- [ ] Filtros.
- [ ] Ordenamiento.
- [ ] Vista de detalles.
- [ ] Creación de órdenes.
- [ ] Integración de pagos.
- [ ] Historial del cliente.
- [ ] Administración de transacciones.
- [ ] Validaciones del lado del servidor.
- [ ] Manejo de errores.
- [ ] Mensajes de confirmación.
- [ ] Diseño con Bootstrap.

---

# 15. Reglas de negocio obligatorias

La aplicación debe impedir:

- [ ] Crear órdenes sin iniciar sesión.
- [ ] Consultar órdenes pertenecientes a otro usuario.
- [ ] Procesar montos iguales o menores que cero.
- [ ] Confirmar una compra sin pago aprobado.
- [ ] Registrar dos veces la misma transacción.
- [ ] Seleccionar vuelos inexistentes.
- [ ] Comprar una tarifa inexistente.
- [ ] Comprar un vuelo no disponible, cuando corresponda.
- [ ] Acceder a funciones administrativas sin autorización.
- [ ] Alterar el precio desde el navegador.
- [ ] Publicar credenciales en GitHub.

## 15.1 Validaciones específicas recomendadas para Tipo 1

- [ ] Origen obligatorio.
- [ ] Destino obligatorio.
- [ ] Origen diferente de destino.
- [ ] Fecha válida.
- [ ] Vuelo existente.
- [ ] Tarifa existente.
- [ ] Monto mayor que cero.
- [ ] Usuario autenticado.
- [ ] Orden perteneciente al usuario.
- [ ] Pago asociado a una orden real.
- [ ] Moneda válida.
- [ ] Identificador externo no repetido.

---

# 16. Seguridad

## 16.1 Almacenamiento de credenciales

Las credenciales deben almacenarse mediante:

- [ ] Variables de entorno.
- [ ] Secret Manager.
- [ ] Archivos de configuración excluidos del repositorio.

## 16.2 Información prohibida en GitHub

No publicar:

- [ ] `ClientSecret`
- [ ] `AccessToken`
- [ ] `StoreID` privado
- [ ] Contraseñas
- [ ] Cadenas de conexión reales
- [ ] Tokens de sesión
- [ ] Claves de API

## 16.3 Archivo de ejemplo

- [ ] Incluir `appsettings.Example.json`.
- [ ] Usar valores ficticios o marcadores.
- [ ] Explicar cómo completar la configuración local.
- [ ] Confirmar que `.gitignore` excluya los archivos sensibles.

## 16.4 Controles adicionales de alta prioridad

- [ ] Usar protección antiforgery en formularios POST.
- [ ] Usar validación de modelo con `ModelState`.
- [ ] Verificar propiedad de la orden en el backend.
- [ ] Aplicar autorización por rol en controladores administrativos.
- [ ] No registrar secretos en logs.
- [ ] No devolver detalles internos de excepciones al usuario.
- [ ] Validar callbacks o respuestas de la pasarela.

---

# 17. Pruebas obligatorias

Deben demostrarse y documentarse las siguientes 16 pruebas:

| N.º | Prueba | Resultado exigido | Estado |
|---:|---|---|---|
| 1 | Instalación y conexión con Airport | Conexión funcional y consulta real | [ ] |
| 2 | Consulta de datos reales | Datos obtenidos desde Airport | [ ] |
| 3 | Registro de usuario | Usuario persistido en Identity | [ ] |
| 4 | Inicio y cierre de sesión | Cookies y sesión funcionales | [ ] |
| 5 | Acceso mediante roles | Menú y rutas según rol | [ ] |
| 6 | Intento de acceso no autorizado | Acceso denegado o redirección correcta | [ ] |
| 7 | Búsqueda con filtros | Resultados correctos | [ ] |
| 8 | Paginación desde PostgreSQL | `Skip` y `Take` ejecutados en BD | [ ] |
| 9 | Creación de una orden | Orden `Pendiente` persistida | [ ] |
| 10 | Pago aprobado | Orden y pago actualizados a `Aprobado` | [ ] |
| 11 | Pago cancelado, rechazado o fallido | Estado registrado correctamente | [ ] |
| 12 | Registro de la transacción | Transacción persistida | [ ] |
| 13 | Prevención de duplicados | Segundo registro rechazado o ignorado | [ ] |
| 14 | Historial del usuario | Solo registros propios | [ ] |
| 15 | Consulta administrativa | Administrador visualiza operaciones globales | [ ] |
| 16 | Protección de credenciales | Repositorio sin secretos | [ ] |

## 17.1 Pruebas adicionales para demostrar dominio del Tipo 1

- [ ] Origen igual a destino.
- [ ] Fecha inválida.
- [ ] Vuelo inexistente.
- [ ] Tarifa modificada desde DevTools.
- [ ] Monto modificado desde el navegador.
- [ ] Usuario intenta abrir una orden ajena.
- [ ] Cliente intenta abrir módulo administrativo.
- [ ] Callback repetido de la pasarela.
- [ ] Pago aprobado genera un solo boleto.
- [ ] Pago no aprobado no genera boleto.

---

# 18. Entregables

Se debe entregar:

- [ ] Código fuente funcional.
- [ ] Enlace público del repositorio GitHub.
- [ ] Informe técnico en PDF.
- [ ] Archivo `README.md`.
- [ ] Migraciones de Entity Framework Core.
- [ ] Scripts SQL adicionales, cuando correspondan.
- [ ] Evidencias de la pasarela.
- [ ] Evidencias de ejecución.

## 18.1 Control de entrega en plataforma

- [ ] El archivo final es PDF.
- [ ] Solo se sube un archivo.
- [ ] El PDF pesa menos de 20 MB.
- [ ] El PDF abre correctamente.
- [ ] El enlace de GitHub es público y funciona.
- [ ] El repositorio puede clonarse.
- [ ] Las instrucciones del README permiten ejecutar el proyecto.
- [ ] No existen secretos en el historial visible del repositorio.

---

# 19. Informe técnico

El informe debe seguir la estructura utilizada durante el semestre.

## 19.1 Portada

Debe incluir:

- [ ] Nombre de la institución.
- [ ] Asignatura.
- [ ] Nombre completo.
- [ ] Tipo de examen asignado: `Tipo 1`.
- [ ] Nombre del ejercicio: `Compra de boletos`.
- [ ] Pasarela utilizada.
- [ ] Fecha.
- [ ] Enlace de GitHub.

## 19.2 Introducción

Debe explicar:

- [ ] En qué consiste la compra de boletos.
- [ ] Qué problema resuelve.
- [ ] Qué tecnologías se utilizaron.
- [ ] Qué parte de Airport se empleó.

## 19.3 Objetivos

- [ ] Un objetivo general.
- [ ] Objetivos específicos.

## 19.4 Desarrollo

Debe documentar:

- [ ] Instalación de Airport.
- [ ] Tablas de Airport utilizadas.
- [ ] Tablas adicionales creadas.
- [ ] Arquitectura del proyecto.
- [ ] Configuración de Entity Framework Core.
- [ ] Configuración de Identity.
- [ ] Roles y permisos.
- [ ] Consultas LINQ.
- [ ] Implementación de la paginación.
- [ ] Proceso de creación de órdenes.
- [ ] Integración de la pasarela.
- [ ] Verificación del pago.
- [ ] Registro de transacciones.
- [ ] Registro del boleto adquirido.
- [ ] Validaciones.
- [ ] Manejo de errores.

## 19.5 Evidencias

Cada evidencia debe contener:

- [ ] Número de figura.
- [ ] Título.
- [ ] Captura legible.
- [ ] Descripción.
- [ ] Explicación de lo demostrado.

Formato esperado:

```text
Figura N. Título descriptivo.

La figura presenta...
```

Ejemplo adaptado:

```text
Figura 5. Listado paginado de vuelos disponibles.

La figura presenta la consulta de vuelos utilizando origen, destino y fecha.
Los registros se recuperan desde PostgreSQL mediante Skip y Take, evitando
cargar la tabla completa en memoria.
```

## 19.6 Fragmentos de código

Los fragmentos deben:

- [ ] Presentarse como texto.
- [ ] No presentarse como capturas.
- [ ] Indicar nombre del archivo.
- [ ] Indicar método o clase.
- [ ] Mostrar únicamente el código principal.
- [ ] Explicar su función.
- [ ] Evitar copiar archivos completos.

## 19.7 Pruebas en el informe

Cada prueba debe documentar:

- [ ] Caso probado.
- [ ] Datos utilizados.
- [ ] Resultado esperado.
- [ ] Resultado obtenido.
- [ ] Evidencia.

## 19.8 Problemas encontrados

Documentar:

- [ ] Problema presentado.
- [ ] Causa identificada.
- [ ] Solución aplicada.
- [ ] Aprendizaje obtenido.

## 19.9 Conclusiones

Las conclusiones deben relacionarse con:

- [ ] Identity.
- [ ] PostgreSQL.
- [ ] Paginación.
- [ ] Entity Framework Core.
- [ ] Pasarela de pago.
- [ ] Seguridad.
- [ ] Compra de boletos.

---

# 20. Repositorio GitHub

## 20.1 Contenido obligatorio

- [ ] Código fuente.
- [ ] Archivo de solución.
- [ ] Migraciones.
- [ ] `README.md`.
- [ ] Instrucciones de ejecución.
- [ ] Dependencias.
- [ ] Configuración necesaria.
- [ ] Descripción del Tipo 1.
- [ ] Pasarela utilizada.
- [ ] Capturas principales.
- [ ] Archivo de configuración de ejemplo.

## 20.2 Contenido prohibido

- [ ] No subir `bin`.
- [ ] No subir `obj`.
- [ ] No subir `.vs`.
- [ ] No subir contraseñas.
- [ ] No subir tokens.
- [ ] No subir `ClientSecret`.
- [ ] No subir `AccessToken`.
- [ ] No subir cadenas de conexión reales.

## 20.3 README mínimo

El README debe incluir:

- [ ] Nombre del proyecto.
- [ ] Descripción del ejercicio.
- [ ] Tipo asignado.
- [ ] Tecnologías.
- [ ] Requisitos previos.
- [ ] Instalación de Airport.
- [ ] Configuración de PostgreSQL.
- [ ] Configuración de secretos.
- [ ] Ejecución de migraciones.
- [ ] Comando para ejecutar la aplicación.
- [ ] Roles disponibles.
- [ ] Credenciales de demostración seguras o instrucciones para crearlas.
- [ ] Flujo de compra.
- [ ] Configuración de la pasarela Sandbox.
- [ ] Capturas principales.
- [ ] Enlace o explicación de la sustentación.

---

# 21. Sustentación en video

Durante la sustentación se debe demostrar:

- [ ] Ejecución del proyecto.
- [ ] Inicio de sesión.
- [ ] Roles.
- [ ] Consulta de Airport.
- [ ] Paginación.
- [ ] Flujo completo de Compra de boletos.
- [ ] Creación de la orden.
- [ ] Pago en Sandbox.
- [ ] Registro de la transacción.
- [ ] Registro del boleto adquirido.
- [ ] Historial del usuario.
- [ ] Código principal.
- [ ] Tablas utilizadas.
- [ ] Código desarrollado.

## 21.1 Preguntas técnicas esperadas

Preparar respuestas sobre:

- [ ] Arquitectura MVC.
- [ ] Entity Framework Core.
- [ ] Database First.
- [ ] Migraciones.
- [ ] Identity.
- [ ] Cookies.
- [ ] LINQ.
- [ ] `Skip()` y `Take()`.
- [ ] Autorización.
- [ ] Seguridad.
- [ ] Flujo de pago.
- [ ] Verificación del pago.
- [ ] Prevención de duplicados.
- [ ] Tablas utilizadas.
- [ ] Código desarrollado.

## 21.2 Respuestas técnicas que deben dominarse

- [ ] Explicar por qué la paginación es física.
- [ ] Explicar por qué `ToListAsync()` se ejecuta al final.
- [ ] Explicar cómo se obtiene el usuario autenticado.
- [ ] Explicar cómo se restringen las órdenes por `UserId`.
- [ ] Explicar cómo se protege el módulo administrativo.
- [ ] Explicar cómo se recalcula el precio en el servidor.
- [ ] Explicar cómo se verifica el pago.
- [ ] Explicar cómo se impide registrar una transacción dos veces.
- [ ] Explicar por qué no se publican secretos.
- [ ] Explicar qué tablas pertenecen a Airport y cuáles son propias.

---

# 22. Rúbrica oficial y estrategia para 20/20

| Criterio | Puntaje | Evidencia indispensable |
|---|---:|---|
| Instalación, conexión y utilización de Airport | 2 | Importación, consulta real y uso en la aplicación |
| Entity Framework Core, modelos y relaciones | 2 | Scaffold, contextos, relaciones y migraciones |
| Identity, sesiones, roles y autorización | 3 | Registro, login, logout, cookies, roles y acceso denegado |
| Desarrollo completo del ejercicio asignado | 3 | Flujo completo de compra de boletos |
| Consultas LINQ, filtros y paginación física | 3 | Matriz LINQ, filtros y SQL paginado |
| Integración funcional de PayPal o PayPhone | 5 | Pago real en Sandbox, verificación y estados |
| Órdenes, pagos, validaciones y seguridad | 1 | Persistencia, reglas, idempotencia y secretos protegidos |
| Informe, GitHub, interfaz y sustentación | 1 | PDF, repositorio público, Bootstrap y video |
| **Total** | **20** | Cumplimiento completo |

## 22.1 Prioridad de implementación según puntaje

### Prioridad 1 — Pasarela funcional: 5 puntos

- [ ] Crear orden pendiente.
- [ ] Crear solicitud en Sandbox.
- [ ] Completar pago.
- [ ] Verificar desde backend.
- [ ] Registrar transacción.
- [ ] Actualizar estado.
- [ ] Manejar cancelación o fallo.
- [ ] Impedir duplicados.
- [ ] Mostrar comprobante.
- [ ] Mostrar historial.

### Prioridad 2 — Identity, roles y autorización: 3 puntos

- [ ] Registro.
- [ ] Login.
- [ ] Logout.
- [ ] Cookies.
- [ ] Roles.
- [ ] Acceso denegado.
- [ ] Historial aislado por usuario.
- [ ] Módulo administrador protegido.

### Prioridad 3 — Compra de boletos completa: 3 puntos

- [ ] Origen.
- [ ] Destino.
- [ ] Fecha.
- [ ] Búsqueda.
- [ ] Filtros.
- [ ] Detalle.
- [ ] Tarifa.
- [ ] Orden.
- [ ] Pago.
- [ ] Boleto.
- [ ] Comprobante.

### Prioridad 4 — LINQ y paginación física: 3 puntos

- [ ] Búsqueda por texto.
- [ ] Dos filtros.
- [ ] Ordenamiento.
- [ ] Relaciones.
- [ ] Proyección.
- [ ] Conteo.
- [ ] Fecha.
- [ ] Estado.
- [ ] `Skip()` y `Take()` en PostgreSQL.

### Prioridad 5 — Airport y EF Core: 4 puntos

- [ ] Airport instalada.
- [ ] Datos reales.
- [ ] Contexto generado.
- [ ] Relaciones verificadas.
- [ ] Migraciones propias.

---

# 23. Plan de implementación recomendado

## Fase 1. Infraestructura

- [ ] Crear repositorio.
- [ ] Configurar `.gitignore`.
- [ ] Crear proyecto ASP.NET Core MVC.
- [ ] Configurar PostgreSQL.
- [ ] Importar Airport.
- [ ] Verificar `bookings.flights`.
- [ ] Agregar Npgsql.
- [ ] Generar `AirportContext`.
- [ ] Configurar Identity.
- [ ] Crear migración inicial.

## Fase 2. Seguridad y usuarios

- [ ] Registro.
- [ ] Login.
- [ ] Logout.
- [ ] Roles.
- [ ] Usuario administrador.
- [ ] Página de acceso denegado.
- [ ] Menús por rol.

## Fase 3. Consulta de vuelos

- [ ] Formulario origen-destino-fecha.
- [ ] Búsqueda real.
- [ ] Dos filtros.
- [ ] Ordenamiento.
- [ ] Proyección.
- [ ] Paginación física.
- [ ] Detalle del vuelo.

## Fase 4. Órdenes

- [ ] Tarifas.
- [ ] Cálculo del servidor.
- [ ] Orden pendiente.
- [ ] Detalle de orden.
- [ ] Validaciones de propiedad.
- [ ] Historial inicial.

## Fase 5. Pago

- [ ] Configurar Sandbox.
- [ ] Crear solicitud.
- [ ] Completar pago.
- [ ] Verificar backend.
- [ ] Registrar pago.
- [ ] Registrar transacción.
- [ ] Manejar estados.
- [ ] Prevenir duplicados.

## Fase 6. Boleto y comprobante

- [ ] Registrar boleto comprado.
- [ ] Generar comprobante.
- [ ] Mostrar historial.
- [ ] Vista administrativa.

## Fase 7. Pruebas y evidencia

- [ ] Ejecutar las 16 pruebas obligatorias.
- [ ] Ejecutar pruebas negativas.
- [ ] Capturar evidencias legibles.
- [ ] Registrar resultados esperados y obtenidos.
- [ ] Verificar seguridad.

## Fase 8. Entrega

- [ ] Completar README.
- [ ] Completar informe.
- [ ] Grabar sustentación.
- [ ] Probar clonación limpia.
- [ ] Revisar secretos.
- [ ] Generar PDF.
- [ ] Verificar tamaño.
- [ ] Subir antes de la fecha límite.

---

# 24. Matriz de trazabilidad

| ID | Requisito | Código | Prueba | Evidencia informe | Estado |
|---|---|---|---|---|---|
| RF-01 | Selección de origen |  |  |  | [ ] |
| RF-02 | Selección de destino |  |  |  | [ ] |
| RF-03 | Selección de fecha |  |  |  | [ ] |
| RF-04 | Búsqueda de vuelos |  |  |  | [ ] |
| RF-05 | Filtros |  |  |  | [ ] |
| RF-06 | Ordenamiento |  |  |  | [ ] |
| RF-07 | Detalle del vuelo |  |  |  | [ ] |
| RF-08 | Selección de tarifa |  |  |  | [ ] |
| RF-09 | Creación de orden |  |  |  | [ ] |
| RF-10 | Pago Sandbox |  |  |  | [ ] |
| RF-11 | Registro del boleto |  |  |  | [ ] |
| RF-12 | Comprobante |  |  |  | [ ] |
| RF-13 | Historial del cliente |  |  |  | [ ] |
| RF-14 | Administración |  |  |  | [ ] |
| RN-01 | Monto calculado en servidor |  |  |  | [ ] |
| RN-02 | Prohibir órdenes anónimas |  |  |  | [ ] |
| RN-03 | Aislamiento por usuario |  |  |  | [ ] |
| RN-04 | Idempotencia de pago |  |  |  | [ ] |
| RN-05 | Compra solo con pago aprobado |  |  |  | [ ] |
| SEC-01 | Secretos fuera de GitHub |  |  |  | [ ] |
| PAG-01 | Paginación física |  |  |  | [ ] |
| AUTH-01 | Roles y autorización |  |  |  | [ ] |

---

# 25. Lista de evidencias recomendada

- [ ] Figura 1. Base Airport instalada.
- [ ] Figura 2. Consulta real a `bookings.flights`.
- [ ] Figura 3. Estructura del proyecto.
- [ ] Figura 4. Registro de usuario.
- [ ] Figura 5. Inicio de sesión.
- [ ] Figura 6. Menú del Cliente.
- [ ] Figura 7. Menú del Administrador.
- [ ] Figura 8. Acceso denegado.
- [ ] Figura 9. Formulario de búsqueda.
- [ ] Figura 10. Resultados filtrados.
- [ ] Figura 11. Paginación física.
- [ ] Figura 12. Detalle del vuelo.
- [ ] Figura 13. Selección de tarifa.
- [ ] Figura 14. Orden pendiente.
- [ ] Figura 15. Solicitud de pago Sandbox.
- [ ] Figura 16. Pago aprobado.
- [ ] Figura 17. Pago cancelado o fallido.
- [ ] Figura 18. Transacción registrada.
- [ ] Figura 19. Prevención de duplicados.
- [ ] Figura 20. Boleto adquirido.
- [ ] Figura 21. Comprobante.
- [ ] Figura 22. Historial del cliente.
- [ ] Figura 23. Consulta administrativa.
- [ ] Figura 24. Archivo de configuración de ejemplo.
- [ ] Figura 25. Repositorio sin secretos.

---

# 26. Fragmentos de código que conviene documentar

- [ ] Configuración de `AirportContext`.
- [ ] Configuración de Identity.
- [ ] Creación de roles.
- [ ] Consulta LINQ de vuelos.
- [ ] Paginación con `Skip()` y `Take()`.
- [ ] Proyección a ViewModel.
- [ ] Validación del usuario autenticado.
- [ ] Creación de orden.
- [ ] Cálculo del monto en el servidor.
- [ ] Creación de solicitud de pago.
- [ ] Verificación del pago.
- [ ] Registro de `Payment`.
- [ ] Registro de `TransactionHistory`.
- [ ] Restricción de duplicados.
- [ ] Registro del boleto.
- [ ] Autorización del administrador.
- [ ] Manejo global o controlado de errores.

---

# 27. Puerta de aprobación antes de entregar

No entregar mientras exista una respuesta negativa.

## Aplicación

- [ ] ¿Compila sin errores?
- [ ] ¿Ejecuta sin errores?
- [ ] ¿Consulta Airport?
- [ ] ¿Usa datos reales?
- [ ] ¿Permite registro, login y logout?
- [ ] ¿Los roles funcionan?
- [ ] ¿Un cliente no puede entrar como administrador?
- [ ] ¿Un cliente no puede consultar órdenes ajenas?
- [ ] ¿La búsqueda origen-destino-fecha funciona?
- [ ] ¿Existen dos filtros?
- [ ] ¿Existe ordenamiento?
- [ ] ¿La paginación se ejecuta en PostgreSQL?
- [ ] ¿La tarifa se recalcula en el servidor?
- [ ] ¿La orden se crea como pendiente?
- [ ] ¿El pago Sandbox es real?
- [ ] ¿El backend verifica el pago?
- [ ] ¿Se registra la transacción?
- [ ] ¿Se evita el duplicado?
- [ ] ¿Se registra el boleto solo con pago aprobado?
- [ ] ¿Se muestra el comprobante?
- [ ] ¿Se muestra el historial?
- [ ] ¿Existe consulta administrativa?
- [ ] ¿Se manejan errores?

## Seguridad

- [ ] ¿No hay contraseñas en GitHub?
- [ ] ¿No hay tokens en GitHub?
- [ ] ¿No hay cadenas reales en GitHub?
- [ ] ¿Existe `appsettings.Example.json`?
- [ ] ¿Los archivos sensibles están ignorados?
- [ ] ¿No aparecen secretos en el historial de commits?

## Documentación

- [ ] ¿La portada contiene todos los datos?
- [ ] ¿El informe documenta cada sección exigida?
- [ ] ¿Las figuras tienen número, título y explicación?
- [ ] ¿El código se presenta como texto?
- [ ] ¿Las pruebas incluyen esperado y obtenido?
- [ ] ¿Las conclusiones cubren todos los temas?
- [ ] ¿El README permite instalar y ejecutar?
- [ ] ¿El video demuestra el flujo completo?

## Entrega

- [ ] ¿El repositorio es público?
- [ ] ¿El enlace funciona?
- [ ] ¿El PDF abre?
- [ ] ¿El PDF pesa menos de 20 MB?
- [ ] ¿Solo se subirá un archivo?
- [ ] ¿La entrega se realizará antes del domingo 2 de agosto de 2026 a las 17:00?

---

# 28. Criterio final de cumplimiento

Un requisito se considera terminado únicamente cuando cumple las tres condiciones:

1. **Implementado:** existe código funcional.
2. **Probado:** existe una prueba con resultado correcto.
3. **Documentado:** existe evidencia en el informe, README o sustentación.

> [!IMPORTANT]
> La mayor concentración de puntaje está en la integración funcional de la pasarela, Identity, el flujo completo de compra y la paginación física. Una interfaz visualmente correcta no compensa un pago simulado, una autorización incompleta o una paginación ejecutada en memoria.
