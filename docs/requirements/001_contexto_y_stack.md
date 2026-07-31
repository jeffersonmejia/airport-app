# 001. Contexto y stack — Examen Airport, Tipo 1: Compra de boletos

> Este archivo forma parte de la división de `requirements.md`.
> Se omite la parte de documentación (informe, README, sustentación, evidencias) por gestión del estudiante.
> **Un requisito solo se marca como cumplido cuando existe implementación funcional, evidencia verificable y documentación correspondiente.**

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

## 2. Resultado final obligatorio

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

## 3. Tecnologías obligatorias

### 3.1 Stack mínimo

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

### 3.2 Capacidades técnicas que deben demostrarse

- [ ] Arquitectura hexagonal con vertical slices y screaming architecture (carpetas nombradas por el negocio: `Flights`, `Bookings`, `Auth`, ...).
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

## 4. Funcionalidades comunes obligatorias

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

## 5. Criterio final de cumplimiento

Un requisito se considera terminado únicamente cuando cumple las tres condiciones:

1. **Implementado:** existe código funcional.
2. **Probado:** existe una prueba con resultado correcto. Incluye **tests unitarios automatizados** para la lógica de negocio pura (ver `006_pruebas_y_estrategia.md`).
3. **Documentado:** existe evidencia en el informe, README o sustentación.

> [!IMPORTANT]
> La mayor concentración de puntaje está en la integración funcional de la pasarela, Identity, el flujo completo de compra y la paginación física. Una interfaz visualmente correcta no compensa un pago simulado, una autorización incompleta o una paginación ejecutada en memoria.
