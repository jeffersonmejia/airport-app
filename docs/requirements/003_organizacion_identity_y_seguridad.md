# 003. Organización del proyecto, Identity y seguridad

> Este archivo forma parte de la división de `requirements.md`.

---

## 1. Organización obligatoria del proyecto

### 1.1 Carpetas mínimas

- [ ] `Controllers`
- [ ] `Models`
- [ ] `Views`
- [ ] `Data`
- [ ] `Services`
- [ ] `ViewModels`
- [ ] `Migrations`
- [ ] `wwwroot`

### 1.2 Separación de lógica

La solución debe separar:

- [ ] Acceso a datos.
- [ ] Reglas del proceso de compra.
- [ ] Gestión de pagos.
- [ ] Autenticación.
- [ ] Presentación de información.
- [ ] Validación de formularios.

> [!CAUTION]
> No debe colocarse toda la lógica dentro de una sola acción del controlador.

### 1.3 Separación mínima recomendada para evitar penalizaciones

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
