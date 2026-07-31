# Feature Administration

Administration contiene el resumen global protegido para `Admin`. El slice
`GetDatabaseSummary` consulta estimaciones del catálogo PostgreSQL mediante EF Core,
las conserva 30 segundos en caché y expone únicamente etiquetas funcionales.

La presentación web muestra el total aproximado y un gráfico circular accesible. La
identidad y los roles siguen perteneciendo exclusivamente a la feature `Auth`.
