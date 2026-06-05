# HallucinationLab

Proyecto de experimento para comparar:

- Salida baseline de un modelo existente.
- La misma salida despues de pasar por un guard basado en OntologiaVacuidad.

Incluye un `ReplayModelBackend` y un guard de control (`PassThroughGuard`) para comparar contra el guard ontologico (`OntologiaOutputGuard`).

El experimento trabaja por casos con:

- `truth`: referencia de verdad canonica (por ejemplo `Francia:capital:París`).
- `toleranciaDefase` y `factorUmbralMagnitud`: parametros del mismo criterio usado en `Models.Tests/AITestHelpers.Alucina`.
- `referenciaPromptVerdad` y `referenciaRespuestaPrompt`: mapeos temporales usados por el criterio AM-FM del test.
- `expectedHallucination`: etiqueta esperada por escenario para medir exactitud frente al test.
- `id`: ahora codifica escenario + motivo esperado + factor de umbral para facilitar lectura de reportes (por ejemplo `c10-hall-mucho-relleno-claim-lyon-f5`).

La deteccion de alucinacion replica la logica de `Alucina` de tests, incluyendo umbral de magnitud y tolerancia de fase.

## Ejecucion rapida

```bash
dotnet run --project HallucinationLab -- --guard pass
```

Opciones utiles:

- `--guard pass|ontologia`

El archivo `hallucination-report.json` se escribe en la raiz del repo.

## Reportes de referencia (dataset de 13 casos)

- `--guard pass`: accuracy vs expected 100%, avg hallucination 84.62%.
- `--guard ontologia`: accuracy vs expected 15.38%, avg hallucination 0%.

Esta diferencia aparece porque la abstencion (`Me abstengo...`) se evalua como no alucinacion, mientras que varias etiquetas `expectedHallucination=true` corresponden a la salida original antes de aplicar el guard.

## Estructura

- `Backends/`: fuente de respuestas (`ReplayModelBackend`).
- `Guard/`: post-procesado (`PassThroughGuard` y `OntologiaOutputGuard`).
- `Eval/`: scoring de alucinacion y metricas agregadas.
- `Core/`: orquestacion del experimento.
