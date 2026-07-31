# Preferencias de trabajo y contexto del proyecto

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

## Versionamiento

debes poner en agents, que se debe mandar commit semantico español, con feat: docs: ... fix, solo esos tres, ah y refactor: ... y solo si yo lo digo.
