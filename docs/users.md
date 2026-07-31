# Usuarios de demostración

Estas cuentas pertenecen al dump local de la asignatura; no se crean mediante un
seeder. Cada contraseña fue verificada contra el hash ya almacenado en
`airportdb.employee`.

| Perfil | Usuario | Contraseña | Roles emitidos |
|---|---|---|---|
| Contabilidad | `Michael1` | `Michael` | `Accounting` |
| Operaciones de pista | `JonathanTaylor5` | `Jonathan Taylor` | `AirfieldOperations` |
| Logística | `David3` | `David` | `Logistics` |
| Marketing | `Greg6` | `Greg` | `Marketing` |
| Administración | `Lauren7` | `Lauren` | `Management`, `Admin` |

`Management` y `Admin` comparten una cuenta de demostración porque `Admin` es un rol
derivado: todo empleado del departamento `Management` recibe ambos claims.

## Inicio de sesión

Enviar una petición a `POST /api/auth/login`:

```json
{
  "username": "Lauren7",
  "password": "Lauren"
}
```

La respuesta incluye el access token JWT, su vencimiento, el usuario y sus roles.
Una nueva autenticación invalida la sesión anterior del mismo empleado.

## Advertencia

Son credenciales académicas de demostración y utilizan el hash legado del dump. No
deben reutilizarse ni publicarse como credenciales de un entorno real. Antes de una
puesta en producción se debe migrar a un algoritmo moderno con salt y eliminar las
contraseñas de este documento.
