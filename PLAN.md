# Plan de implementación — Airport DB y API .NET

## 1. Estado inicial verificado

| Elemento | Estado |
|---|---|
| Dump | `aeropuerto-db.sql`, formato SQL plano UTF-8 |
| Tamaño | 2,103,567,296 bytes (aprox. 2.0 GiB) |
| Origen del dump | PostgreSQL 18.4 / `pg_dump` 18.4 |
| Finalización del dump | Completo: contiene `PostgreSQL database dump complete` |
| Esquema | `airportdb`, 14 tablas |
| PostgreSQL local | Cliente `psql` 17.10; servidor sin respuesta en `localhost:5432` al revisar |
| .NET SDK | 10.0.301 |
| Equipo | 2 núcleos AMD 3020e, 5.7 GiB RAM, 5.9 GiB swap |
| Disco disponible | Aproximadamente 701 GiB |

Tablas detectadas: `airline`, `airplane`, `airplane_type`, `airport`,
`airport_geo`, `airport_reachable`, `booking`, `employee`, `flight`,
`flight_log`, `flightschedule`, `passenger`, `passengerdetails` y
`weatherdata`.

## 2. Decisiones técnicas

1. PostgreSQL se ejecutará como servicio nativo del sistema. Docker queda fuera de
   esta primera implementación.
2. Se debe preferir PostgreSQL 18 nativo porque el dump fue producido por 18.4.
   Restaurarlo en 17 puede funcionar parcialmente, pero no es una combinación
   garantizada. Primero se comprobarán los clusters instalados y su versión real.
3. El rol administrador `postgres` realizará la restauración. La API usará un rol
   separado llamado `postgresql`, con contraseña `postgresql` según el requisito,
   sin privilegios de superusuario.
4. Nombre propuesto para la base: `airport_exam`; el esquema interno seguirá siendo
   `airportdb`.
5. La contraseña no se guardará en archivos versionados. En desarrollo se usará
   `dotnet user-secrets`, una variable de entorno o un archivo `.pgpass` con permisos
   `0600`.
6. La carga será secuencial, reanudable y sin paralelismo. Los ciclos se harán dentro
   de los bloques `COPY`, no sólo entre tablas, porque una sola tabla puede ocupar
   gran parte de los 2 GiB.
7. La API no insertará los datos masivos mediante EF Core. El dump se restaurará con
   las herramientas nativas de PostgreSQL y la aplicación sólo consumirá la base.
8. El trabajo será paralelo en el calendario, no simultáneo sobre el CPU: durante un
   ciclo de importación no se compilará el proyecto; las tareas de .NET se ejecutarán
   durante las pausas del importador.

## 3. Estrategia de importación de bajo consumo

### Ciclo propuesto

- Procesar entre 50,000 y 100,000 filas por transacción `COPY`.
- Confirmar el bloque y registrar tabla, número de bloque, filas y desplazamiento del
  archivo fuente.
- Descansar 20–30 segundos.
- Continuar sólo si la memoria disponible es al menos 800 MiB, el sistema no está
  aumentando swap de forma sostenida y la carga promedio no supera aproximadamente
  1.5 en los dos núcleos.
- Si el equipo se mantiene estable durante tres ciclos, aumentar gradualmente el
  bloque; si empieza a usar swap o se vuelve poco responsivo, reducirlo a la mitad y
  aumentar la pausa.

`nice` e `ionice` reducirán la prioridad del lector, pero no limitan por sí solos el
backend de PostgreSQL. La protección real será: un solo proceso, cero construcción
paralela de índices, bloques acotados, commits y pausas explícitas.

El importador que se implemente después leerá el archivo de forma secuencial y con
memoria constante. Cada fila de un `COPY` de texto ocupa una línea lógica; los saltos
de línea contenidos en valores están escapados, por lo que se puede cerrar un bloque
con `\.` y repetir la misma sentencia `COPY` para el bloque siguiente. No se cargará
el dump completo en RAM ni se generará una segunda copia permanente de 2 GiB.

## 4. Fases de ejecución

### Fase 0 — Preparación y protección

- [ ] Registrar checksum SHA-256, tamaño y fecha del dump usando baja prioridad.
- [ ] Confirmar clusters instalados, versión del servidor y estado del servicio.
- [ ] Levantar PostgreSQL nativo en `localhost:5432`.
- [ ] Usar PostgreSQL 18; si sólo existe 17, instalar/crear un cluster 18 antes de la
      carga definitiva.
- [ ] Verificar que no exista una base `airport_exam` con datos que deban conservarse.
- [ ] Comprobar al menos 15 GiB libres para datos, índices, temporales y WAL. El equipo
      actualmente supera ampliamente este margen.
- [ ] Cerrar aplicaciones pesadas y medir una línea base de RAM, swap, carga y disco.

**Criterio de salida:** servidor compatible activo, dump intacto y destino seguro.

### Fase 1 — Roles, base y seguridad

- [ ] Crear o ajustar el rol login `postgresql` sin `SUPERUSER`, `CREATEDB`,
      `CREATEROLE` ni `REPLICATION`.
- [ ] Crear `airport_exam` con UTF-8 y propietario administrativo controlado.
- [ ] Restaurar como `postgres`; después conceder a `postgresql` sólo conexión, uso
      del esquema y permisos CRUD/secuencias requeridos por la API.
- [ ] Configurar credenciales fuera del repositorio y probar conexión del rol de la
      aplicación.

**Criterio de salida:** el administrador puede restaurar y el usuario de aplicación
no tiene privilegios globales.

### Fase 2 — Analizador y manifiesto del dump

- [ ] Implementar un analizador streaming que distinga: pre-data, cada bloque `COPY`,
      ajustes de secuencias y post-data.
- [ ] Crear un manifiesto con tablas, columnas, posiciones de inicio/fin y conteos por
      bloques, sin retener los datos en memoria.
- [ ] Crear un estado reanudable local en `.state/import-state.json`.
- [ ] Validar el analizador primero con una muestra pequeña y una base temporal.
- [ ] Mantener `ON_ERROR_STOP=1`; un bloque con error no se marcará como terminado.

**Criterio de salida:** una interrupción permite reanudar desde el último bloque
confirmado sin duplicar filas.

### Fase 3 — Creación de estructura

- [ ] Ejecutar sólo tipos, esquema, tablas y secuencias de la sección pre-data.
- [ ] No crear aún índices secundarios, claves foráneas ni constraints costosos.
- [ ] Verificar las 14 tablas, columnas, tipos enum y secuencias.
- [ ] Aplicar límites de sesión conservadores: `work_mem` cercano a 4 MiB,
      `maintenance_work_mem` entre 64 y 128 MiB y
      `max_parallel_maintenance_workers=0`.

**Criterio de salida:** estructura vacía y válida, sin operaciones pesadas pendientes
en segundo plano.

### Fase 4 — Carga de datos por ciclos

- [ ] Cargar una sola tabla y un solo bloque a la vez mediante `COPY`.
- [ ] Empezar con 50,000 filas y pausa de 30 segundos durante los primeros ciclos.
- [ ] Registrar duración, filas, memoria disponible, swap y carga después de cada
      commit.
- [ ] Adaptar tamaño y descanso según las métricas; nunca ejecutar dos cargas en
      paralelo.
- [ ] Permitir pausa manual limpia después de cualquier bloque confirmado.
- [ ] En una reanudación, verificar el estado contra la base antes de continuar.

**Criterio de salida:** todos los bloques del manifiesto están confirmados y los
conteos por tabla coinciden.

### Fase 5 — Secuencias, índices y relaciones

- [ ] Aplicar los valores `setval` de las secuencias.
- [ ] Crear claves primarias e índices uno por uno, con paralelismo desactivado y una
      pausa entre objetos pesados.
- [ ] Crear y validar claves foráneas al final.
- [ ] Ejecutar `ANALYZE` tabla por tabla, no un mantenimiento global paralelo.
- [ ] Revisar índices inválidos y constraints no validados.

**Criterio de salida:** integridad referencial completa, secuencias correctas y
estadísticas disponibles para el optimizador.

### Fase 6 — Validación de la base

- [ ] Comparar los conteos reales con el manifiesto.
- [ ] Verificar tamaños por tabla y tamaño total con `pg_total_relation_size`.
- [ ] Probar relaciones representativas: vuelos–aerolínea–avión,
      reservas–pasajeros y aeropuertos–geolocalización.
- [ ] Confirmar que el rol `postgresql` puede consultar y modificar únicamente lo
      esperado.
- [ ] Guardar un reporte de importación con duración, errores recuperados y checksum.
- [ ] Realizar un respaldo nuevo en formato custom cuando la base quede validada.

**Criterio de salida:** restauración reproducible, consistente y accesible para la API.

## 5. Plan del proyecto .NET

Se usará .NET 10 con ASP.NET Core y un monolito modular. La arquitectura combina:

- **Screaming architecture:** los nombres principales representan negocio
  (`Flights`, `Bookings`, `Passengers`, `Airports`, etc.).
- **Vertical slices:** cada caso de uso contiene endpoint, contrato, validación y
  handler, en lugar de carpetas globales de controllers/services.
- **Hexagonal:** el dominio y los casos de uso no dependen de PostgreSQL ni de HTTP;
  los adaptadores implementan sus puertos.

Estructura objetivo:

```text
Airport.sln
src/
  Airport.Api/                         # Adaptador HTTP y composition root
  Airport.Core/
    Flights/
      Domain/
      Features/
        GetFlight/
        SearchFlights/
    Bookings/
      Domain/
      Features/
        CreateBooking/
        GetBooking/
    Passengers/
    Airports/
    Airlines/
    Fleet/
    Employees/
    Weather/
    Shared/
      Ports/
  Airport.Infrastructure/              # Adaptadores PostgreSQL, tiempo, logging
    Persistence/
    Features/
      Flights/
      Bookings/
      Passengers/
      Airports/
tests/
  Airport.UnitTests/
  Airport.IntegrationTests/
tools/
  import/                              # Importador reanudable y manifiesto
```

Reglas de dependencia:

```text
Airport.Api -------------> Airport.Core
Airport.Infrastructure --> Airport.Core
Airport.Api -------------> Airport.Infrastructure  (sólo para composición/DI)
Airport.Core ------------> ninguna capa externa
```

### Orden de creación

- [ ] Crear solución y proyectos con `dotnet new`, sin lógica de negocio inicial.
- [ ] Añadir referencias respetando las reglas anteriores.
- [ ] Configurar nullable, warnings y formato común.
- [ ] Añadir PostgreSQL mediante Npgsql/EF Core en Infrastructure; la versión exacta
      se elegirá verificando compatibilidad con .NET 10.
- [ ] Configurar la conexión con user-secrets y health check de PostgreSQL.
- [ ] Implementar primero un slice vertical de lectura (`Flights/GetFlight`) como
      patrón de referencia.
- [ ] Implementar pruebas unitarias del Core y pruebas de integración contra la base
      local.
- [ ] Incorporar los demás slices por prioridad del examen.

No se usarán migraciones para recrear ni poblar los 2 GiB. Si más adelante la API
necesita cambios propios de esquema, sus migraciones empezarán desde una línea base
posterior a la importación.

## 6. Coordinación entre importación y .NET

| Momento | PostgreSQL | Trabajo .NET |
|---|---|---|
| Ciclo activo | Importar un bloque | Sólo edición ligera |
| Pausa de descanso | Sin carga | Scaffold, restore o build de un proyecto |
| Índice/constraint | Operación secuencial | Sin compilación ni tests |
| Pausa larga/manual | En reposo | Ejecutar tests y análisis |

## 7. Condición de terminado

- La base `airport_exam` contiene las 14 tablas y todos los datos del dump.
- Conteos, secuencias, constraints e índices pasan la validación.
- La carga puede interrumpirse y reanudarse por bloque.
- La API .NET 10 compila, conecta con el rol `postgresql` y mantiene las dependencias
  hexagonales.
- Existe al menos un vertical slice funcional con prueba unitaria e integración.
- No hay dumps, contraseñas, artefactos de build ni estado del importador en Git.
