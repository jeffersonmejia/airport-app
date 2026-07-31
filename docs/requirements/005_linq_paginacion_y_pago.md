# 005. LINQ, paginación física y flujo de pago

> Este archivo forma parte de la división de `requirements.md`.

---

## 1. Consultas LINQ obligatorias

La aplicación debe utilizar LINQ para recuperar, filtrar, ordenar y proyectar información.

### 1.1 Operadores que deben utilizarse según corresponda

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

### 1.2 Evidencias mínimas exigidas

| Requisito LINQ | Implementación propuesta | Estado |
|---|---|---|
| Búsqueda por texto | Número de vuelo, aeropuerto o ciudad | [ ] |
| Filtro 1 | Origen o destino | [ ] |
| Filtro 2 | Estado o fecha | [ ] |
| Ordenamiento | Salida programada ascendente o descendente | [ ] |
| Consulta con relaciones | Vuelo con aeropuerto, ruta o aeronave | [ ] |
| Proyección con `Select()` | Response del slice de búsqueda (`SearchFlightsResponse`) | [ ] |
| Conteo o totalización | Total de vuelos o total de ventas | [ ] |
| Consulta por fecha | Vuelos de la fecha seleccionada | [ ] |
| Consulta por estado | Scheduled, Departed, Cancelled u otro estado real | [ ] |
| Consulta paginada | `Skip()` y `Take()` antes de `ToListAsync()` | [ ] |

### 1.3 Control estricto

- [ ] Las consultas se ejecutan en PostgreSQL.
- [ ] No se llama `ToListAsync()` antes de filtrar y paginar.
- [ ] Se utiliza `AsNoTracking()` en consultas de solo lectura.
- [ ] Se proyecta únicamente la información necesaria.

---

## 2. Paginación física obligatoria

### 2.1 Reglas

- [ ] Recuperar los registros poco a poco desde PostgreSQL.
- [ ] Contar los registros con `CountAsync()`.
- [ ] Ordenar antes de paginar.
- [ ] Aplicar `Skip()`.
- [ ] Aplicar `Take()`.
- [ ] Ejecutar `ToListAsync()` después de `Skip()` y `Take()`.
- [ ] No cargar todos los registros para paginar en memoria.

#### Patrón correcto

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

#### Patrón prohibido

```csharp
var todos = await consulta.ToListAsync();

var paginaActual = todos
    .Skip((pagina - 1) * tamanioPagina)
    .Take(tamanioPagina);
```

### 2.2 Información visible en cada listado principal

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

## 3. Flujo obligatorio de pago

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

### 3.1 Condiciones no negociables

- [ ] La pasarela forma parte del flujo real del ejercicio.
- [ ] No basta con colocar un botón.
- [ ] No basta con mostrar una pantalla simulada.
- [ ] El monto se recalcula en el servidor.
- [ ] El backend verifica la transacción.
- [ ] El pago se registra en PostgreSQL.
- [ ] El boleto se emite únicamente tras aprobación.
- [ ] El resultado fallido o cancelado también se registra correctamente.
