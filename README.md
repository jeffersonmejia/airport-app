# Airport App

Aplicación académica en .NET 10 con ASP.NET Core, Razor/Blazor, EF Core, Npgsql y
arquitectura feature-first.

## Políticas principales

- Toda paginación admite como máximo **5 registros**.
- Las consultas de lectura usan caché en memoria: TTL de **30 segundos** y límite de
  **256 entradas**. Las futuras escrituras deben invalidar sus claves relacionadas.
- Auth usa JWT Bearer con access tokens de **15 minutos** y sin refresh token por
  ahora.
- Sólo se permite una sesión por usuario. Cada token incluye un `jti`; al iniciar una
  nueva sesión se reemplaza la anterior y su token deja de ser válido.
- La sesión activa vive en memoria. Reiniciar la API invalida los tokens existentes;
  si la aplicación escala a varias instancias deberá migrarse a caché distribuida.

## Configuración local segura

La API lee desde .NET User Secrets:

- `ConnectionStrings:AirportDb`
- `Auth:Jwt:Issuer`
- `Auth:Jwt:Audience`
- `Auth:Jwt:SigningKey`
- `Auth:Jwt:MinimumAccessTokenMinutes`
- `Auth:Jwt:MaximumAccessTokenMinutes`
- `Auth:Jwt:AccessTokenMinutes`
- `Auth:Jwt:ClockSkewSeconds`

Los valores reales no se guardan en Git. La restauración de PostgreSQL se administra
por separado mediante `run.txt`.

Las URLs públicas no son secretos. En desarrollo, `Airport.Web` obtiene
`ApiBaseUrl` desde `wwwroot/appsettings.Development.json` y `Airport.Api` obtiene los
orígenes permitidos desde `appsettings.Development.json`. Para producción se deben
proporcionar `ApiBaseUrl` y `Cors:AllowedOrigins` con los dominios reales del entorno.
