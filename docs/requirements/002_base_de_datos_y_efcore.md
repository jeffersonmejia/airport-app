# 002. Base de datos Airport y Entity Framework Core

> Este archivo forma parte de la división de `requirements.md`.

---

## 1. Base de datos Airport

### 1.1 Instalación obligatoria

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

#### Comando de referencia

```powershell
& "C:\Program Files\PostgreSQL\17\bin\psql.exe" `
-h localhost `
-p 5432 `
-U postgres `
-d postgres `
-v ON_ERROR_STOP=1 `
-f "C:\BD\airport\demo-20250901-3m.sql"
```

#### Verificación mínima

```sql
SELECT current_database();

SELECT COUNT(*)
FROM bookings.flights;
```

### 1.2 Restricciones de uso

- [ ] Utilizar datos reales de Airport.
- [ ] Conservar la estructura original de Airport.
- [ ] No alterar destructivamente las tablas originales.
- [ ] Crear tablas propias para las operaciones de la aplicación.
- [ ] Utilizar únicamente las entidades de Airport necesarias para el Tipo 1.

---

## 2. Entidades requeridas para el Tipo 1

### 2.1 Tablas de Airport sugeridas

- [ ] `airports_data`
- [ ] `flights`
- [ ] `routes`
- [ ] `bookings`
- [ ] `tickets`
- [ ] `ticket_flights`

> Las tablas anteriores son sugeridas por el enunciado. Deben seleccionarse las necesarias para implementar correctamente el flujo de compra.

### 2.2 Tablas propias sugeridas

- [ ] `Orders`
- [ ] `OrderDetails`
- [ ] `PurchasedTickets`
- [ ] `Payments`
- [ ] `TransactionHistory`

### 2.3 Tablas adicionales mínimas equivalentes

El proyecto debe crear tablas equivalentes a:

- [ ] `Orders`
- [ ] `OrderDetails`
- [ ] `Payments`
- [ ] `TransactionHistory`

### 2.4 Campos mínimos de `Payments`

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

#### Restricción crítica

- [ ] `ExternalTransactionId` debe utilizarse para impedir el registro duplicado de una misma transacción.
- [ ] Debe existir una restricción única o una validación equivalente que garantice la idempotencia.
- [ ] La prevención de duplicados debe demostrarse mediante una prueba.

---

## 3. Entity Framework Core

### 3.1 Conexión y proveedor

- [ ] Instalar y configurar `Npgsql.EntityFrameworkCore.PostgreSQL`.
- [ ] Configurar la cadena de conexión sin publicarla en GitHub.
- [ ] Conectar la aplicación con Airport.
- [ ] Ejecutar una consulta real desde Entity Framework Core.

### 3.2 Database First

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

### 3.3 Migraciones

- [ ] Crear las tablas propias mediante migraciones.
- [ ] Incluir las migraciones en el repositorio.
- [ ] Aplicar las migraciones a PostgreSQL.
- [ ] Confirmar que una instalación limpia pueda ejecutar las migraciones.

```powershell
dotnet ef migrations add InitialApplicationTables
dotnet ef database update
```

### 3.4 Contextos

Se permite:

- [ ] Un contexto para Airport.
- [ ] Otro contexto para Identity y tablas propias.

Debe definirse:

- [ ] Qué contexto administra cada conjunto de entidades.
- [ ] Cómo se configuran ambos contextos.
- [ ] Cómo se evita mezclar responsabilidades.
