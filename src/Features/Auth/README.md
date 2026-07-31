# Feature Auth — esqueleto

Auth es una feature independiente. Ya configura JWT Bearer, emisión de access tokens
y una sola sesión activa por usuario. Login y lectura de credenciales siguen como
esqueleto.

Slices previstos:

- `Login`: validar credenciales y crear una sesión o emitir un token.
- `Logout`: invalidar la sesión activa cuando el mecanismo elegido lo permita.
- `GetCurrentUser`: devolver la identidad autenticada y sus permisos.

Se usarán access tokens de 15 minutos, sin refresh token por ahora. Antes de Login se
deben definir roles y el tratamiento seguro de `airportdb.employee.password`. La clave
no se comparará ni almacenará en texto plano.

Reglas de dependencia:

```text
Presentation/Api ----> Application ----> Domain
Presentation/Web ----------------------> contratos HTTP
Infrastructure ------> Application ----> Domain
```
