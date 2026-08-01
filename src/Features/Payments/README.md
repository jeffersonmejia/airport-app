# Feature Payments

Payments encapsula la integración con PayPal Sandbox mediante arquitectura hexagonal
y vertical slices.

- `CreatePayPalOrder`: crea una orden con intención `CAPTURE`.
- `CapturePayPalOrder`: captura una orden después de la aprobación del pagador.
- `IPayPalGateway`: puerto definido por Application para comunicarse con PayPal.
- `IPaymentOrderStore`: puerto que obtiene la orden y persiste el pago confirmado.
- `PayPalPaymentGateway`: adaptador HTTP de Infrastructure para Orders API v2.

Los endpoints requieren la política `ClientOnly` y el encabezado
`PayPal-Request-Id` para idempotencia. Las credenciales se leen desde
`PayPal:ClientId` y `PayPal:ClientSecret` en User Secrets. El access token OAuth se
obtiene y reutiliza en memoria hasta su expiración; nunca se envía al navegador ni
se persiste.

El navegador solo envía el identificador de la orden. El backend recupera el monto
y la moneda persistidos, verifica que la captura coincida exactamente y, dentro de
una transacción, registra el pago, marca la orden como pagada y emite el boleto.
