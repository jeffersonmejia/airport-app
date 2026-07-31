# 006. Pruebas, tests unitarios y estrategia

> Este archivo forma parte de la división de `requirements.md`.

---

## 1. Pruebas obligatorias

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

### 1.1 Pruebas adicionales para demostrar dominio del Tipo 1

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

## 2. Tests unitarios obligatorios

Además de las pruebas funcionales (manuales o de integración), **se deben escribir tests unitarios automatizados** que cubran la lógica pura del negocio. Recomendado: **xUnit** (consistente con el repositorio actual en `tests/Airport.UnitTests`).

Deben ejecutarse con `dotnet test` y cubrir, como mínimo:

### 2.1 Cálculo de tarifas y montos

- [ ] Cálculo correcto de la tarifa según la regla de negocio.
- [ ] Monto mayor que cero.
- [ ] Rechazo de montos iguales o menores que cero.
- [ ] Aplicación de moneda válida.
- [ ] El precio se calcula en el servidor (no se confía en el navegador).

### 2.2 Validaciones

- [ ] Origen obligatorio.
- [ ] Destino obligatorio.
- [ ] Origen diferente de destino.
- [ ] Fecha válida y con formato correcto.
- [ ] Vuelo inexistente → rechazo.
- [ ] Tarifa inexistente → rechazo.

### 2.3 Estados y transiciones

- [ ] Orden nueva queda `Pendiente`.
- [ ] Pago aprobado → orden `Aprobado`.
- [ ] Cancelación → `Cancelado`.
- [ ] Rechazo de la pasarela → `Rechazado`.
- [ ] Error técnico → `Fallido`.
- [ ] No se confirma compra sin pago aprobado.
- [ ] No se emite boleto sin pago aprobado.
- [ ] Pago aprobado genera un solo boleto.

### 2.4 Idempotencia y reglas de negocio

- [ ] Mismo `ExternalTransactionId` no se registra dos veces.
- [ ] Callback repetido de la pasarela → segundo registro rechazado o ignorado.
- [ ] No se crea orden sin usuario autenticado.
- [ ] Un cliente no accede a órdenes de otro usuario.

> [!IMPORTANT]
> Los tests unitarios deben ser deterministas y no depender de la pasarela ni de la base de datos real. La lógica de negocio debe aislarse en servicios para poder probarla sin infraestructura.

---

## 3. Rúbrica oficial y estrategia para 20/20

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

### 3.1 Prioridad de implementación según puntaje

#### Prioridad 1 — Pasarela funcional: 5 puntos

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

#### Prioridad 2 — Identity, roles y autorización: 3 puntos

- [ ] Registro.
- [ ] Login.
- [ ] Logout.
- [ ] Cookies.
- [ ] Roles.
- [ ] Acceso denegado.
- [ ] Historial aislado por usuario.
- [ ] Módulo administrador protegido.

#### Prioridad 3 — Compra de boletos completa: 3 puntos

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

#### Prioridad 4 — LINQ y paginación física: 3 puntos

- [ ] Búsqueda por texto.
- [ ] Dos filtros.
- [ ] Ordenamiento.
- [ ] Relaciones.
- [ ] Proyección.
- [ ] Conteo.
- [ ] Fecha.
- [ ] Estado.
- [ ] `Skip()` y `Take()` en PostgreSQL.

#### Prioridad 5 — Airport y EF Core: 4 puntos

- [ ] Airport instalada.
- [ ] Datos reales.
- [ ] Contexto generado.
- [ ] Relaciones verificadas.
- [ ] Migraciones propias.

---

## 4. Plan de implementación recomendado

### Fase 1. Infraestructura

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

### Fase 2. Seguridad y usuarios

- [ ] Registro.
- [ ] Login.
- [ ] Logout.
- [ ] Roles.
- [ ] Usuario administrador.
- [ ] Página de acceso denegado.
- [ ] Menús por rol.

### Fase 3. Consulta de vuelos

- [ ] Formulario origen-destino-fecha.
- [ ] Búsqueda real.
- [ ] Dos filtros.
- [ ] Ordenamiento.
- [ ] Proyección.
- [ ] Paginación física.
- [ ] Detalle del vuelo.

### Fase 4. Órdenes

- [ ] Tarifas.
- [ ] Cálculo del servidor.
- [ ] Orden pendiente.
- [ ] Detalle de orden.
- [ ] Validaciones de propiedad.
- [ ] Historial inicial.

### Fase 5. Pago

- [ ] Configurar Sandbox.
- [ ] Crear solicitud.
- [ ] Completar pago.
- [ ] Verificar backend.
- [ ] Registrar pago.
- [ ] Registrar transacción.
- [ ] Manejar estados.
- [ ] Prevenir duplicados.

### Fase 6. Boleto y comprobante

- [ ] Registrar boleto comprado.
- [ ] Generar comprobante.
- [ ] Mostrar historial.
- [ ] Vista administrativa.

### Fase 7. Pruebas y evidencia

- [ ] Ejecutar las 16 pruebas obligatorias.
- [ ] Ejecutar pruebas negativas.
- [ ] Escribir los tests unitarios de la sección 2.
- [ ] Ejecutar `dotnet test` en verde.
- [ ] Capturar evidencias legibles.
- [ ] Registrar resultados esperados y obtenidos.
- [ ] Verificar seguridad.

### Fase 8. Entrega

- [ ] Completar README.
- [ ] Completar informe.
- [ ] Grabar sustentación.
- [ ] Probar clonación limpia.
- [ ] Revisar secretos.
- [ ] Generar PDF.
- [ ] Verificar tamaño.
- [ ] Subir antes de la fecha límite.

---

## 5. Matriz de trazabilidad

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

## 6. Puerta de aprobación antes de entregar

No entregar mientras exista una respuesta negativa.

### Aplicación

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
- [ ] ¿Los tests unitarios pasan con `dotnet test`?

### Seguridad

- [ ] ¿No hay contraseñas en GitHub?
- [ ] ¿No hay tokens en GitHub?
- [ ] ¿No hay cadenas reales en GitHub?
- [ ] ¿Existe `appsettings.Example.json`?
- [ ] ¿Los archivos sensibles están ignorados?
- [ ] ¿No aparecen secretos en el historial de commits?
