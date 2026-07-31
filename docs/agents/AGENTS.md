# Preferencias de trabajo y contexto del proyecto

## Inicio de un agente o chat nuevo

- Todo agente nuevo, incluyendo Codex, OpenCode u otra herramienta equivalente, debe leer completamente `requirements.md`, ubicado en esta misma carpeta, antes de analizar, planificar o modificar el proyecto.
- Esta lectura también es obligatoria al iniciar un chat nuevo, aunque el agente ya conozca parcialmente el proyecto.
- Después debe consultar `DIAGRAMS.md` cuando necesite comprender la arquitectura, los módulos, las relaciones o los flujos del sistema.
- Si `requirements.md` está vacío, debe trabajar a partir de la solicitud actual del usuario y del estado real del repositorio, sin inventar requisitos.

## Flujo de trabajo

1. Leer `requirements.md` y las instrucciones aplicables.
2. Revisar el estado actual del repositorio y los archivos relacionados con la tarea.
3. Implementar primero los cambios solicitados en el código.
4. Compilar y ejecutar las pruebas seguras que correspondan.
5. Informar qué se cambió y el resultado de las verificaciones.
6. Si conviene reiniciar un componente, sugerirlo y esperar la autorización expresa del usuario antes de hacerlo.

## Forma de trabajo preferida

- Primero se implementan los cambios solicitados en el código.
- Después de codificar, se informa qué se cambió y se sugieren los reinicios que sean convenientes.
- No se debe reiniciar la aplicación, la API, el frontend, los contenedores ni el servicio de base de datos sin autorización expresa del usuario.
- Si un reinicio es necesario o recomendable para probar o aplicar los cambios, se debe solicitar confirmación antes de ejecutarlo.
- Se permite consultar y modificar la base de datos cuando sea necesario para cumplir una tarea autorizada, incluyendo cambios de esquema, datos y migraciones relacionados con esa tarea.
- No se debe reiniciar, restablecer, vaciar, eliminar ni recrear la base de datos sin autorización expresa. Estas acciones se deben proponer primero, explicando brevemente su impacto, y esperar la confirmación del usuario.
- Siempre se deben conservar los datos existentes, salvo que el usuario autorice explícitamente una operación destructiva.

## Contexto técnico

- Es una aplicación académica de aeropuerto desarrollada con .NET 10 y ASP.NET Core.
- Utiliza Entity Framework Core con PostgreSQL mediante Npgsql.
- La interfaz web usa Razor/Blazor.
- La solución principal es `Airport.sln`.
- Los hosts ejecutables son `src/Hosts/Airport.Api` y `src/Hosts/Airport.Web`.
- El proyecto está organizado actualmente con arquitectura feature-first y separación entre Domain, Application, Infrastructure y Presentation.
- Las credenciales y la cadena de conexión deben mantenerse fuera del repositorio y administrarse mediante .NET User Secrets o configuración segura equivalente.

## Verificación

- Se pueden compilar y ejecutar pruebas sin pedir autorización, siempre que esto no reinicie servicios ni realice cambios destructivos en la base de datos.
- Al terminar una modificación, se deben comunicar el resultado de la compilación y las pruebas realizadas.
- Si para verificar el cambio hace falta reiniciar algún componente, se debe sugerir el comando o la acción y esperar la autorización del usuario.
