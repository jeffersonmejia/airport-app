# 001 — Contexto, alcance y decisiones

## Estado inicial verificado

| Elemento | Estado |
|---|---|
| Dump | `aeropuerto-db.sql`, SQL plano UTF-8 |
| Tamaño | 2,103,567,296 bytes (aprox. 2.0 GiB) |
| Origen | PostgreSQL 18.4 / `pg_dump` 18.4 |
| Integridad básica | Completo: contiene `PostgreSQL database dump complete` |
| Esquema | `airportdb`, 14 tablas |
| PostgreSQL local | Cliente 17.10; el servidor no respondía en `localhost:5432` durante la revisión |
| .NET SDK | 10.0.301 |
| Equipo | 2 núcleos AMD 3020e, 5.7 GiB RAM y 5.9 GiB swap |
| Disco disponible | Aproximadamente 701 GiB |

Las tablas detectadas son `airline`, `airplane`, `airplane_type`, `airport`,
`airport_geo`, `airport_reachable`, `booking`, `employee`, `flight`,
`flight_log`, `flightschedule`, `passenger`, `passengerdetails` y
`weatherdata`.

## Alcance inicial

El primer entregable será un sistema aeroportuario ejecutado directamente en el
equipo, compuesto por:

- PostgreSQL nativo con la información proporcionada por el examen.
- Backend ASP.NET Core sobre .NET 10.
- Arquitectura hexagonal, screaming architecture y vertical slices.
- Interfaz web minimalista basada en Material Design.
- Pruebas unitarias y de integración para los primeros casos de uso.

Docker queda fuera de esta etapa. Se evaluará después de tener la base y la
aplicación funcionando de forma nativa, para evitar el costo de recrear contenedores
durante el desarrollo inicial.

## Decisiones principales

1. Se preferirá PostgreSQL 18 nativo porque el dump fue generado con la versión
   18.4. No se hará la carga definitiva en PostgreSQL 17 sin validar compatibilidad.
2. La base se llamará `airport_exam` y conservará el esquema `airportdb`.
3. La restauración se ejecutará con el usuario administrador `postgres`; la clave
   indicada para el entorno local es `postgres`.
4. El usuario ejecutará personalmente la creación y restauración siguiendo
   `run.txt`. El desarrollo de la aplicación comenzará después de confirmar que la
   carga terminó correctamente.
5. Las credenciales de la aplicación no se escribirán en archivos versionados. Se
   utilizarán user-secrets o variables de entorno.
6. La carga masiva no se realizará con EF Core: PostgreSQL importará el dump y la
   aplicación consumirá la base ya creada.
7. El backend será un monolito modular. Los nombres y carpetas expresarán primero el
   negocio y cada operación se organizará como un vertical slice.
8. El estilo visual seguirá Material Design con rosa pastel, composición minimalista,
   iconografía Material y animaciones suaves.

## Orden general

1. Crear y validar la base siguiendo `run.txt`.
2. Generar la solución y sus límites arquitectónicos.
3. Conectar Infrastructure con PostgreSQL.
4. Implementar un slice vertical completo como referencia.
5. Crear el shell visual y el sistema de diseño.
6. Incorporar features y pruebas por prioridad del examen.
7. Validar rendimiento, accesibilidad y documentación.
