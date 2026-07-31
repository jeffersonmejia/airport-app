# 004 — Experiencia visual y entrega

## Dirección visual

La interfaz seguirá Material Design con una identidad rosa pastel inspirada en la
energía visual de PedidosYa, sin copiar su marca. El resultado debe sentirse limpio,
amable y ligero: pocos elementos por pantalla, jerarquía clara y abundante espacio
en blanco.

Principios:

- Minimalismo funcional: cada pantalla destaca una acción principal.
- Material Design como base de componentes, estados y accesibilidad.
- Rosa pastel como acento, no como fondo dominante de todas las superficies.
- Iconografía consistente de Material Symbols Rounded.
- Animaciones breves y suaves que expliquen cambios de estado.
- Diseño responsive para escritorio, tablet y móvil.

## Paleta inicial

| Token | Color | Uso |
|---|---|---|
| Primary | `#E85D8E` | Acciones principales y selección |
| Primary container | `#FFD9E4` | Chips, tarjetas destacadas y estados suaves |
| On primary | `#FFFFFF` | Texto e iconos sobre primary |
| Secondary | `#7C5360` | Acciones secundarias y metadatos |
| Background | `#FFF7F9` | Fondo general |
| Surface | `#FFFFFF` | Tarjetas, diálogos y navegación |
| Surface variant | `#F7E8ED` | Separación sutil de secciones |
| Text primary | `#24191D` | Texto principal |
| Text secondary | `#6B5860` | Texto auxiliar |
| Success | `#3A7D5D` | Confirmaciones y estados correctos |
| Error | `#BA1A1A` | Errores y acciones destructivas |

La paleta deberá verificarse con contraste WCAG antes de cerrar los componentes. Los
colores no serán el único mecanismo para comunicar estados.

## Componentes y composición

- Barra superior compacta con identidad de Airport y acciones globales.
- Navegación lateral en escritorio y navegación adaptable en pantallas pequeñas.
- Tarjetas de vuelos con ruta, horarios, estado y aerolínea claramente jerarquizados.
- Búsqueda prominente con filtros convertidos en chips.
- Tablas únicamente cuando la comparación lo requiera; en móvil se transformarán en
  listas o tarjetas.
- Botones con etiquetas claras; los icon-only buttons siempre tendrán tooltip y
  nombre accesible.
- Bordes redondeados de 12–16 px, elevación discreta y cuadrícula de espaciado de 8 px.
- Estados vacíos, carga, error y éxito diseñados desde el primer slice visual.

## Iconografía

Se utilizará una sola familia: Material Symbols Rounded. Ejemplos previstos:

| Acción o concepto | Icono sugerido |
|---|---|
| Vuelos | `flight` |
| Aeropuertos | `location_on` |
| Reservas | `confirmation_number` |
| Pasajeros | `group` |
| Horarios | `schedule` |
| Clima | `partly_cloudy_day` |
| Buscar | `search` |
| Filtros | `filter_list` |

Los iconos acompañarán texto cuando la acción pueda ser ambigua. No se mezclarán
familias outline, filled y rounded sin una regla de estado explícita.

## Movimiento e interacción

- Duración estándar entre 160 y 240 ms.
- Curvas de aceleración suaves tipo Material para entrada, salida y transformación.
- Transiciones cortas en hover, selección de chips, expansión de filtros y aparición
  de resultados.
- Skeletons discretos durante consultas; no usar spinners para toda la pantalla salvo
  en una carga inicial inevitable.
- Evitar rebotes, desplazamientos largos y animaciones decorativas continuas.
- Respetar `prefers-reduced-motion` y eliminar movimiento no esencial cuando esté
  activo.

## Implementación por etapas

- [x] Definir tokens de color, tipografía, espaciado, radio, elevación y movimiento.
- [x] Crear el shell responsive y la navegación principal.
- [x] Diseñar estados de loading, empty, error y success.
- [x] Construir la búsqueda y detalle de vuelo como primer slice visual.
- [x] Conectar el slice con Airport.Api usando un cliente HTTP y contrato explícito.
- [x] Incorporar estructura semántica, nombres accesibles, foco visible y soporte para
      `prefers-reduced-motion`.
- [ ] Verificar manualmente teclado, lectores de pantalla y contraste.
- [ ] Probar en anchos móvil, tablet y escritorio.
- [x] Centralizar los tokens para reutilizarlos en las features posteriores.

Durante la reorganización feature-first, el shell y los tokens permanecerán en el
host `Airport.Web`. La página, formulario de búsqueda, estados y tarjeta de vuelos se
moverán a `Features/Flights/Presentation/Web`, de modo que la presentación específica
también pertenezca a su feature.

La implementación visual se realizó sin ejecutar restore, build, servidor local ni
pruebas de navegador, de acuerdo con las restricciones de esta etapa.

## Condición global de entrega

- La base está restaurada y validada por el usuario mediante `run.txt`.
- Backend y cliente web compilan con .NET 10.
- La arquitectura conserva sus límites hexagonales y organización por features.
- Al menos un flujo vertical funciona desde la interfaz hasta PostgreSQL.
- La interfaz utiliza la paleta rosa pastel, Material Symbols y movimiento accesible.
- Pruebas unitarias e integración cubren el flujo de referencia.
- Dumps, secretos y artefactos de compilación permanecen fuera de Git.
