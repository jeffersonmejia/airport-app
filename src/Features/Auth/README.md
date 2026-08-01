# Feature Auth

Auth es una feature independiente. Conserva JWT Bearer para los empleados del
esquema legado y añade acceso de clientes con Google mediante ASP.NET Core Identity.

Slices previstos:

- `Login`: implementado; valida credenciales y emite un token con roles.
- `Google`: crea o recupera la cuenta externa y abre una sesión cookie de cliente.
- `MFA`: enlaza una aplicación TOTP mediante QR, verifica códigos y genera códigos
  de recuperación.
- `Session`: devuelve la identidad cookie actual y permite cerrarla.

Se usan access tokens de 15 minutos, sin refresh token por ahora. La compatibilidad
temporal con los usuarios del dump traduce `Management` a los roles `Client` y
`Admin`; los demás departamentos reciben `Client`. El login mantiene compatibilidad
temporal con el hash MD5 del dump; no compara ni almacena contraseñas en texto plano
y ese algoritmo deberá migrarse antes de producción.

Google requiere `Authentication:Google:ClientId` y
`Authentication:Google:ClientSecret` en User Secrets. El callback público se define
en `Authentication:Google:WebCallbackUrl`. Identity utiliza tablas propias dentro
del esquema `airport_app`; deben crearse mediante una migración antes de probar el
flujo Google/MFA.

Reglas de dependencia:

```text
Presentation/Api ----> Application ----> Domain
Presentation/Web ----------------------> contratos HTTP
Infrastructure ------> Application ----> Domain
```
