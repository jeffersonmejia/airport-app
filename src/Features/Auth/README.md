# Feature Auth

Auth es una feature independiente. Configura JWT Bearer, emisión de access tokens,
una sola sesión activa por usuario y login de empleados contra el esquema legado.

Slices previstos:

- `Login`: implementado; valida credenciales y emite un token con roles.
- `Logout`: invalidar la sesión activa cuando el mecanismo elegido lo permita.
- `GetCurrentUser`: devolver la identidad autenticada y sus permisos.

Se usan access tokens de 15 minutos, sin refresh token por ahora. La compatibilidad
temporal con los usuarios del dump traduce `Management` a los roles `Client` y
`Admin`; los demás departamentos reciben `Client`. El login mantiene compatibilidad
temporal con el hash MD5 del dump; no compara ni almacena contraseñas en texto plano
y ese algoritmo deberá migrarse antes de producción.

Reglas de dependencia:

```text
Presentation/Api ----> Application ----> Domain
Presentation/Web ----------------------> contratos HTTP
Infrastructure ------> Application ----> Domain
```
