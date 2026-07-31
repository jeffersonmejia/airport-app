# Preferencias de trabajo y contexto del proyecto

## Forma de trabajo preferida

- Primero se implementan los cambios solicitados en el código.
- Después de codificar, se informa qué se cambió y el resultado de la compilación y las pruebas.
- Los reinicios de la aplicación, la API, el frontend, los contenedores y el servicio de base de datos los realiza el propio usuario; el agente no debe reiniciarlos ni sugerir comandos de reinicio.
- Se permite consultar y modificar la base de datos cuando sea necesario para cumplir una tarea autorizada, incluyendo cambios de esquema, datos y migraciones relacionados con esa tarea.
- No se debe reiniciar, restablecer, vaciar, eliminar ni recrear la base de datos. Estas acciones las propone el agente explicando brevemente su impacto y las ejecuta el usuario.
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
- Si para verificar el cambio hace falta reiniciar algún componente, el agente sólo lo indica en el resumen final; el reinicio lo ejecuta el usuario.

## Versionamiento

debes poner en agents, que se debe mandar commit semantico español, con feat: docs: ... fix, solo esos tres, ah y refactor: ... y solo si yo lo digo.
