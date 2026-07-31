# 002 — Base de datos PostgreSQL

## Responsabilidad y punto de entrada

La creación de la base y la restauración serán ejecutadas manualmente por el usuario.
El único punto de entrada operativo es `run.txt`; este plan no duplica su comando ni
lo sustituye.

Datos acordados para la restauración local:

| Parámetro | Valor |
|---|---|
| Host | `localhost` |
| Puerto | `5432` |
| Base | `airport_exam` |
| Esquema importado | `airportdb` |
| Usuario | `postgres` |
| Contraseña | `postgres` |
| Archivo | `aeropuerto-db.sql` |

La contraseña aparece aquí porque fue indicada expresamente para el entorno local de
examen. No debe reutilizarse en producción ni incluirse en la cadena de conexión que
se publique en Git.

## Preparación

- [ ] Confirmar que PostgreSQL 18 esté instalado como servicio nativo.
- [ ] Confirmar que el servicio escuche en `localhost:5432`.
- [ ] Crear una base vacía llamada `airport_exam`.
- [ ] Verificar que no exista información previa que deba conservarse.
- [ ] Mantener `aeropuerto-db.sql` en la raíz del repositorio; `.gitignore` evita que
      sus 2 GiB entren en Git.
- [ ] Cerrar aplicaciones pesadas antes de iniciar la carga.
- [ ] Conservar al menos 15 GiB disponibles para datos, índices, temporales y WAL.

## Ejecución y recursos

El comando de `run.txt` usa una sola sesión, prioridad reducida, 4 MiB de `work_mem`,
128 MiB de `maintenance_work_mem` y desactiva workers paralelos de mantenimiento.
Esto reduce la competencia por los dos núcleos y la memoria disponible, aunque no
impone pausas físicas al servicio PostgreSQL.

Para el AMD 3020e de dos núcleos y 5.7 GiB de RAM, el tiempo estimado es de 1 a 3
horas. Puede acercarse a 4 horas si el almacenamiento es lento, aumenta el uso de swap
o existen otras aplicaciones consumiendo CPU y disco.

Durante la ejecución:

- [ ] No cerrar la terminal ni suspender el equipo.
- [ ] Evitar compilaciones, tests y aplicaciones pesadas.
- [ ] Vigilar memoria disponible y swap con `free -h` desde otra terminal.
- [ ] Vigilar carga y procesos con `top` o `htop`.
- [ ] Si el equipo deja de responder, registrar el error antes de intentar de nuevo;
      `ON_ERROR_STOP=1` evita continuar silenciosamente tras un fallo SQL.

El dump SQL plano no ofrece reanudación desde el último bloque con el comando actual.
Si la ejecución se interrumpe, se debe limpiar la base parcial, crear nuevamente una
base vacía y repetir la restauración. Esa acción debe hacerse de forma consciente para
no eliminar otra base por error.

## Validación posterior

- [ ] Confirmar que existen las 14 tablas en `airportdb`.
- [ ] Comprobar que la salida terminó sin errores y mostró el final del dump.
- [ ] Revisar constraints e índices inválidos o sin validar.
- [ ] Verificar secuencias y valores máximos de sus claves relacionadas.
- [ ] Ejecutar `ANALYZE` sólo si la restauración no dejó estadísticas adecuadas.
- [ ] Consultar tamaños por tabla y tamaño total con `pg_total_relation_size`.
- [ ] Probar relaciones representativas: vuelos–aerolínea–avión,
      reservas–pasajeros y aeropuertos–geolocalización.
- [ ] Probar una conexión desde .NET usando credenciales almacenadas fuera del
      repositorio.

## Seguridad posterior

El rol `postgres` se utilizará para restauración y administración local. Antes de
conectar la aplicación de forma definitiva, se creará un rol exclusivo sin
`SUPERUSER`, `CREATEDB`, `CREATEROLE` ni `REPLICATION`, con acceso únicamente a
`airport_exam` y al esquema `airportdb`.

## Criterio de terminado

- La restauración termina sin errores.
- Las 14 tablas contienen datos coherentes.
- Índices, constraints y secuencias están válidos.
- La aplicación puede conectarse con un rol limitado.
- El dump y las credenciales continúan fuera del historial de Git.
