# 004. Requerimientos funcionales, reglas de negocio y estados

> Este archivo forma parte de la división de `requirements.md`.

---

## 1. Requerimientos funcionales específicos del Tipo 1

### RF-01. Selección de origen

- [ ] Mostrar aeropuertos reales de Airport.
- [ ] Permitir seleccionar un aeropuerto de origen.
- [ ] Validar que el origen exista.
- [ ] No confiar únicamente en el valor recibido desde el navegador.

**Criterio de aceptación:** el servidor valida el código o identificador contra PostgreSQL.

### RF-02. Selección de destino

- [ ] Mostrar aeropuertos reales de Airport.
- [ ] Permitir seleccionar un aeropuerto de destino.
- [ ] Validar que el destino exista.
- [ ] Impedir que origen y destino sean iguales.

**Criterio de aceptación:** no se procesa una búsqueda inválida.

### RF-03. Fecha

- [ ] Permitir elegir una fecha.
- [ ] Validar el formato.
- [ ] Consultar vuelos para la fecha seleccionada.
- [ ] Incluir una consulta LINQ por fecha.

### RF-04. Búsqueda de vuelos

- [ ] Consultar vuelos reales.
- [ ] Aplicar origen.
- [ ] Aplicar destino.
- [ ] Aplicar fecha.
- [ ] Aplicar los filtros adicionales requeridos.
- [ ] No cargar todos los vuelos en memoria.
- [ ] Ejecutar la consulta paginada en PostgreSQL.

### RF-05. Filtros

- [ ] Incluir una búsqueda por texto.
- [ ] Incluir al menos dos filtros.
- [ ] Conservar los filtros al cambiar de página.
- [ ] Conservar los filtros al cambiar el ordenamiento.

Ejemplos válidos para este ejercicio:

- Texto: número de vuelo, nombre de aeropuerto o ciudad.
- Filtro 1: estado del vuelo.
- Filtro 2: aeropuerto de origen, destino, fecha o aeronave.
- Filtro adicional: franja horaria o disponibilidad.

### RF-06. Ordenamiento

- [ ] Incorporar al menos un ordenamiento.
- [ ] Aplicarlo antes de `Skip()` y `Take()`.
- [ ] Conservarlo durante la navegación de páginas.

Ejemplos:

- Fecha y hora de salida.
- Duración.
- Número de vuelo.
- Tarifa calculada.

### RF-07. Detalle del vuelo

- [ ] Mostrar la información principal del vuelo.
- [ ] Mostrar origen.
- [ ] Mostrar destino.
- [ ] Mostrar fecha y hora programadas.
- [ ] Mostrar estado.
- [ ] Mostrar aeronave o ruta, cuando corresponda.
- [ ] Validar nuevamente la existencia del vuelo en el servidor.

### RF-08. Selección de tarifa

- [ ] Permitir seleccionar una tarifa.
- [ ] Definir las tarifas disponibles en el servidor o base de datos.
- [ ] Calcular el monto en el servidor.
- [ ] No aceptar como confiable un precio enviado desde el navegador.
- [ ] Registrar la tarifa elegida en `OrderDetails`.

### RF-09. Creación de orden

- [ ] Exigir que el usuario esté autenticado.
- [ ] Consultar nuevamente el vuelo seleccionado.
- [ ] Calcular nuevamente el monto.
- [ ] Crear la orden con estado `Pendiente`.
- [ ] Asociar la orden con el usuario autenticado.
- [ ] Registrar el detalle.
- [ ] Guardar fechas de creación y estado.

### RF-10. Pago

- [ ] Crear una solicitud real en Sandbox.
- [ ] Redirigir o integrar el flujo oficial de la pasarela.
- [ ] Verificar el resultado desde el backend.
- [ ] No considerar aprobado un pago basándose únicamente en parámetros del navegador.
- [ ] Registrar el identificador externo.
- [ ] Actualizar orden y pago según el resultado.
- [ ] Manejar aprobación.
- [ ] Manejar cancelación, rechazo o fallo.

### RF-11. Registro del boleto adquirido

- [ ] Registrar el boleto después de un pago aprobado.
- [ ] Asociarlo con la orden.
- [ ] Asociarlo con el usuario.
- [ ] Asociarlo con el vuelo.
- [ ] Registrar la tarifa y el monto.
- [ ] Impedir emitirlo si el pago no está aprobado.
- [ ] Impedir emitirlo dos veces por la misma transacción.

### RF-12. Comprobante

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

## 2. Requisitos transversales conservados

Los siguientes requisitos generales ya existentes se mantienen como soporte del
flujo de compra, pero no amplían el ejercicio a otro tipo del examen.

### RF-13. Historial del cliente

- [ ] Mostrar únicamente las operaciones del usuario autenticado.
- [ ] Mostrar órdenes.
- [ ] Mostrar pagos.
- [ ] Mostrar boletos adquiridos.
- [ ] Permitir consultar el detalle.
- [ ] Proteger contra modificación de identificadores en la URL.

### RF-14. Administración

- [ ] Listar todas las órdenes.
- [ ] Listar todos los pagos.
- [ ] Listar transacciones.
- [ ] Filtrar por usuario.
- [ ] Filtrar por estado.
- [ ] Consultar reportes generales.
- [ ] Proteger todo el módulo con rol `Administrador`.

---

## 3. Reglas de negocio obligatorias

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

### 3.1 Validaciones específicas recomendadas para Tipo 1

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

## 4. Estados mínimos

Las órdenes y pagos deben manejar, como mínimo:

- [ ] `Pendiente`
- [ ] `Aprobado`
- [ ] `Cancelado`
- [ ] `Rechazado`
- [ ] `Fallido`

### 4.1 Transiciones que deben controlarse

- [ ] Orden nueva → `Pendiente`.
- [ ] Pago verificado y aprobado → orden `Aprobado`.
- [ ] Cancelación del usuario → `Cancelado`.
- [ ] Rechazo de la pasarela → `Rechazado`.
- [ ] Error técnico o verificación fallida → `Fallido`.
- [ ] No confirmar la compra sin pago aprobado.
- [ ] No registrar dos veces la misma transacción.
