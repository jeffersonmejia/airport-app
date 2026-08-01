# Feature Payments

Payments encapsula la integración con PayPal Sandbox mediante arquitectura hexagonal
y vertical slices.

- `CreatePayPalOrder`: crea una orden con intención `CAPTURE`.
- `CapturePayPalOrder`: captura una orden después de la aprobación del pagador.
- `IPayPalGateway`: puerto definido por Application.
- `PayPalPaymentGateway`: adaptador HTTP de Infrastructure para Orders API v2.

Los endpoints requieren temporalmente la política `AdminOnly` y el encabezado
`PayPal-Request-Id` para idempotencia. Las credenciales se leen desde `PayPal:ClientId` y
`PayPal:ClientSecret` en User Secrets. El access token OAuth se obtiene y reutiliza
en memoria hasta su expiración; nunca se envía al navegador ni se persiste.

Esta feature no calcula precios ni crea boletos. Mientras no exista la orden
persistida, solo un administrador puede probar la integración Sandbox. Al integrar
el flujo completo, el monto deberá provenir de una orden recalculada por el backend
antes de habilitar estos casos de uso al cliente.
