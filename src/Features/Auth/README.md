# Feature Auth — esqueleto

Auth es una feature independiente. Por ahora sólo define sus fronteras; no contiene
autenticación funcional ni se registra en los hosts.

Slices previstos:

- `Login`: validar credenciales y crear una sesión o emitir un token.
- `Logout`: invalidar la sesión activa cuando el mecanismo elegido lo permita.
- `GetCurrentUser`: devolver la identidad autenticada y sus permisos.

Antes de implementarlos se debe decidir si el examen exige cookies o JWT, la política
de roles y el tratamiento del campo legado `airportdb.employee.password`. La clave no
se comparará ni almacenará en texto plano. Auth será responsable de las credenciales;
Employees conservará la información laboral y personal.

Reglas de dependencia:

```text
Presentation/Api ----> Application ----> Domain
Presentation/Web ----------------------> contratos HTTP
Infrastructure ------> Application ----> Domain
```
