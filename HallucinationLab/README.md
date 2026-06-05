# HallucinationLab

Proyecto de experimento para comparar:

- Salida baseline de un modelo existente.
- La misma salida despues de pasar por un guard basado en OntologiaVacuidad.

Ahora incluye un backend real de OpenAI (`OpenAiResponsesBackend`) y un guard de control (`PassThroughGuard`) para probar que, si no se modifica la salida, la tasa de alucinacion se mantiene.

El experimento trabaja por casos con:

- `truth`: referencia de verdad canonica (por ejemplo `Francia:capital:París`).

La deteccion de alucinacion se calcula sobre cada respuesta generada, en funcion de cuanto cubre la referencia de verdad del caso.

## Ejecucion rapida

```bash
dotnet run --project HallucinationLab -- --cases HallucinationLab/Samples/cases.json --guard pass --out hallucination-report.json
```

Opciones utiles:

- `--guard pass|ontologia`
- `--model gpt-4o-mini`
- `--api-key <key>` (o `OPENAI_API_KEY`)
- `--strict-openai` para fallar si no hay clave

Si no hay `OPENAI_API_KEY`, el programa usa `ReplayModelBackend` automaticamente para que se pueda ejecutar sin configuracion adicional.

## Nota sobre API publica sin credenciales

La API oficial de OpenAI requiere credenciales (retorna `401 Unauthorized` sin API key). Por eso se incluye fallback local para ejecucion sin configuracion.

## Estructura

- `Backends/`: fuente de respuestas (`OpenAiResponsesBackend` y fallback `ReplayModelBackend`).
- `Guard/`: post-procesado (`PassThroughGuard` y `OntologiaOutputGuard`).
- `Eval/`: scoring de alucinacion y metricas agregadas.
- `Core/`: orquestacion del experimento.
