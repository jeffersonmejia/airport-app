# Requerimientos estrictos — Examen Airport, Tipo 1: Compra de boletos

> Lista maestra de implementación, pruebas, documentación, repositorio y sustentación.
>
> **Propósito:** controlar el cumplimiento total del examen y reducir omisiones que puedan afectar la calificación.
>
> **Regla de uso:** un requisito solo se marca como cumplido cuando existe implementación funcional, evidencia verificable y documentación correspondiente.
>
> **Estructura:** este archivo es la lista maestra y **redirige a los subdocumentos** de `docs/requirements/`, donde viven los checklists detallados por área. La documentación de entrega (informe, repositorio, sustentación y evidencias) se mantiene aquí, en las secciones 4 a 9.
>
> **Alcance:** se implementa exclusivamente el ejercicio asignado, **Tipo 1: Compra de boletos**. No se agregarán módulos ni funcionalidades pertenecientes a otros tipos del examen.
>
> **Base de datos:** la instalación, importación y comprobación inicial de Airport ya están cumplidas y se omiten del trabajo pendiente. La aplicación sí debe seguir utilizando esa base existente y respetar los requisitos de acceso, entidades y persistencia propios del Tipo 1.

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

### 2.1 Flujo específico del Tipo 1: Compra de boletos

La aplicación debe permitir:

- [ ] Seleccionar un aeropuerto de origen.
- [ ] Seleccionar un aeropuerto de destino.
- [ ] Elegir una fecha.
- [ ] Buscar vuelos disponibles.
- [ ] Aplicar filtros.
- [ ] Paginar los resultados.
- [ ] Consultar el detalle del vuelo.
- [ ] Seleccionar una tarifa.
- [ ] Crear una orden.
- [ ] Procesar el pago.
- [ ] Registrar el boleto adquirido.
- [ ] Mostrar un comprobante.

### 2.2 Requisitos transversales ya existentes

Se conservan los requisitos generales del examen que respaldan y aseguran el flujo anterior:

- [ ] Registrar usuarios.
- [ ] Iniciar sesión.
- [ ] Cerrar sesión.
- [ ] Aplicar autenticación persistente mediante cookies.
- [ ] Diferenciar los roles `Administrador` y `Cliente`.
- [ ] Proteger rutas y operaciones con autorización.
- [ ] Consultar datos reales de la base Airport.
- [ ] Registrar el detalle de la orden.
- [ ] Aplicar búsqueda y ordenamiento al listado de vuelos.
- [ ] Paginar físicamente los resultados desde PostgreSQL.
- [ ] Procesar el pago mediante PayPal Sandbox o PayPhone en pruebas.
- [ ] Verificar el resultado del pago en el backend.
- [ ] Registrar el pago y la transacción en PostgreSQL.
- [ ] Impedir transacciones duplicadas.
- [ ] Mostrar el historial individual del cliente.
- [ ] Permitir al administrador revisar órdenes, pagos y transacciones.

---

## 3. Redirección a subdocumentos de requerimientos

Los requerimientos detallados se dividen en subdocumentos numerados. Cada subdocumento es la
fuente de verdad de su área y allí se marcan los checklists.

| Subdocumento | Contenido | Cubre las secciones |
|---|---|---|
| [001_contexto_y_stack.md](001_contexto_y_stack.md) | Identificación, resultado final, stack mínimo, capacidades técnicas, funcionalidades comunes y criterio de cumplimiento | 1-3, 14, 28 |
| [002_base_de_datos_y_efcore.md](002_base_de_datos_y_efcore.md) | Base Airport ya preparada, entidades exclusivas del Tipo 1, EF Core, Database First, migraciones y contextos | 4-6 |
| [003_organizacion_identity_y_seguridad.md](003_organizacion_identity_y_seguridad.md) | Organización del proyecto (hexagonal, vertical slices y screaming architecture), Identity, roles y seguridad | 7-8, 16 |
| [004_flujo_compra_y_reglas.md](004_flujo_compra_y_reglas.md) | Requerimientos funcionales RF-01..14, reglas de negocio y estados | 9, 13, 15 |
| [005_linq_paginacion_y_pago.md](005_linq_paginacion_y_pago.md) | Consultas LINQ, paginación física y flujo de pago | 10-12 |
| [006_pruebas_y_estrategia.md](006_pruebas_y_estrategia.md) | Última etapa: pruebas obligatorias, tests unitarios, rúbrica 20/20, matriz de trazabilidad y puerta de aprobación | 17, 22-24, 27 |

> [!IMPORTANT]
> El subdocumento `006_pruebas_y_estrategia.md` se aborda de último, después de completar el trabajo funcional definido en `001` a `005`. Esto no impide realizar verificaciones técnicas puntuales durante el desarrollo.

> [!NOTE]
> La división se hizo en `docs:` commit `1ed6118`; los checklists ya no se duplican en este archivo.

---

## 4. Entregables

Se debe entregar:

- [ ] Código fuente funcional.
- [ ] Enlace público del repositorio GitHub.
- [ ] Informe técnico en PDF.
- [ ] Archivo `README.md`.
- [ ] Migraciones de Entity Framework Core.
- [ ] Scripts SQL adicionales, cuando correspondan.
- [ ] Evidencias de la pasarela.
- [ ] Evidencias de ejecución.

### 4.1 Control de entrega en plataforma

- [ ] El archivo final es PDF.
- [ ] Solo se sube un archivo.
- [ ] El PDF pesa menos de 20 MB.
- [ ] El PDF abre correctamente.
- [ ] El enlace de GitHub es público y funciona.
- [ ] El repositorio puede clonarse.
- [ ] Las instrucciones del README permiten ejecutar el proyecto.
- [ ] No existen secretos en el historial visible del repositorio.

---

## 5. Informe técnico

El informe debe seguir la estructura utilizada durante el semestre.

### 5.1 Portada

Debe incluir:

- [ ] Nombre de la institución.
- [ ] Asignatura.
- [ ] Nombre completo.
- [ ] Tipo de examen asignado: `Tipo 1`.
- [ ] Nombre del ejercicio: `Compra de boletos`.
- [ ] Pasarela utilizada.
- [ ] Fecha.
- [ ] Enlace de GitHub.

### 5.2 Introducción

Debe explicar:

- [ ] En qué consiste la compra de boletos.
- [ ] Qué problema resuelve.
- [ ] Qué tecnologías se utilizaron.
- [ ] Qué parte de Airport se empleó.

### 5.3 Objetivos

- [ ] Un objetivo general.
- [ ] Objetivos específicos.

### 5.4 Desarrollo

Debe documentar:

- [ ] Instalación de Airport.
- [ ] Tablas de Airport utilizadas.
- [ ] Tablas adicionales creadas.
- [ ] Arquitectura del proyecto (hexagonal con vertical slices y screaming architecture).
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

### 5.5 Evidencias

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

### 5.6 Fragmentos de código

Los fragmentos deben:

- [ ] Presentarse como texto.
- [ ] No presentarse como capturas.
- [ ] Indicar nombre del archivo.
- [ ] Indicar método o clase.
- [ ] Mostrar únicamente el código principal.
- [ ] Explicar su función.
- [ ] Evitar copiar archivos completos.

### 5.7 Pruebas en el informe

Cada prueba debe documentar:

- [ ] Caso probado.
- [ ] Datos utilizados.
- [ ] Resultado esperado.
- [ ] Resultado obtenido.
- [ ] Evidencia.

### 5.8 Problemas encontrados

Documentar:

- [ ] Problema presentado.
- [ ] Causa identificada.
- [ ] Solución aplicada.
- [ ] Aprendizaje obtenido.

### 5.9 Conclusiones

Las conclusiones deben relacionarse con:

- [ ] Identity.
- [ ] PostgreSQL.
- [ ] Paginación.
- [ ] Entity Framework Core.
- [ ] Pasarela de pago.
- [ ] Seguridad.
- [ ] Compra de boletos.

---

## 6. Repositorio GitHub

### 6.1 Contenido obligatorio

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

### 6.2 Contenido prohibido

- [ ] No subir `bin`.
- [ ] No subir `obj`.
- [ ] No subir `.vs`.
- [ ] No subir contraseñas.
- [ ] No subir tokens.
- [ ] No subir `ClientSecret`.
- [ ] No subir `AccessToken`.
- [ ] No subir cadenas de conexión reales.

### 6.3 README mínimo

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

## 7. Sustentación en video

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

### 7.1 Preguntas técnicas esperadas

Preparar respuestas sobre:

- [ ] Arquitectura hexagonal, vertical slices y screaming architecture.
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

### 7.2 Respuestas técnicas que deben dominarse

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

## 8. Lista de evidencias recomendada

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

## 9. Fragmentos de código que conviene documentar

- [ ] Configuración de `AirportContext`.
- [ ] Configuración de Identity.
- [ ] Creación de roles.
- [ ] Consulta LINQ de vuelos.
- [ ] Paginación con `Skip()` y `Take()`.
- [ ] Proyección al Response del slice de búsqueda.
- [ ] Validación del usuario autenticado.
- [ ] Creación de orden.
- [ ] Cálculo del monto en el servidor.
- [ ] Creación de solicitud de pago.
- [ ] Verificación del pago.
- [ ] Registro de `Payment`.
- [ ] Registro del pago y su identificador de transacción en `Payments`.
- [ ] Restricción de duplicados.
- [ ] Registro del boleto.
- [ ] Autorización del administrador.
- [ ] Manejo global o controlado de errores.

---

## 10. Criterio final de cumplimiento

Un requisito se considera terminado únicamente cuando cumple las tres condiciones:

1. **Implementado:** existe código funcional.
2. **Probado:** existe una prueba con resultado correcto.
3. **Documentado:** existe evidencia en el informe, README o sustentación.

> [!IMPORTANT]
> La mayor concentración de puntaje está en la integración funcional de la pasarela, Identity, el flujo completo de compra y la paginación física. Una interfaz visualmente correcta no compensa un pago simulado, una autorización incompleta o una paginación ejecutada en memoria.
