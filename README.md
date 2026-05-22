# OntologiaVacuidad

Solución C# para Ontología Vacuidad:

Aplicación de consola y librerías para probar el sistema diseñado en el artículo de Medim titulado "Ontología de la vacuidad: un sistema para entender la realidad como modulación (AM-FM-PM) y plasma primordial (RGB)".

https://medium.com/@heroe.vajradharma/ontolog%C3%ADa-de-la-vacuidad-un-sistema-para-entender-la-realidad-como-modulaci%C3%B3n-am-fm-fsk-y-plasma-2c8b56f2d8ed

## Experimento de alucinaciones

Se agrego el proyecto `HallucinationLab` para comparar:

1. Salida baseline de un modelo existente.
2. La misma salida, postprocesada con un guard ontologico basado en este repositorio.

El flujo permite medir si bajan las alucinaciones con dos listas por caso:

- `expectedFacts`: hechos que la respuesta deberia contener.
- `forbiddenClaims`: afirmaciones consideradas alucinaciones.

### Estructura principal

- `HallucinationLab/Backends`: backend real (`OpenAiResponsesBackend`) y fallback (`ReplayModelBackend`).
- `HallucinationLab/Guard`: guard de control (`PassThroughGuard`) y guard ontologico (`OntologiaOutputGuard`).
- `HallucinationLab/Eval`: scoring y metricas agregadas.
- `HallucinationLab/Samples`: casos y respuestas de ejemplo.

### Uso rapido

Ejecutar desde la raiz de la solucion:

```bash
dotnet run --project HallucinationLab -- --cases HallucinationLab/Samples/cases.json --guard pass --out hallucination-report.json
```

Para usar OpenAI real:

```bash
$env:OPENAI_API_KEY="tu_api_key"
dotnet run --project HallucinationLab -- --cases HallucinationLab/Samples/cases.json --guard pass --model gpt-4o-mini --out hallucination-report.json
```

El comando genera:

- Resumen por consola (baseline vs guarded).
- Archivo `hallucination-report.json` con detalle por caso y metricas agregadas.

### Como conectar tu modelo real

1. Implementa `ITextModelBackend` en `HallucinationLab/Backends` para llamar tu proveedor (OpenAI, Azure, local, etc.).
2. Mantene el mismo prompt para baseline y guarded.
3. Usa el baseline como salida original del modelo.
4. Aplica `OntologiaOutputGuard` para obtener la salida modificada.
5. Compara metricas para estimar reduccion de alucinaciones.

### Nota sobre OpenAI sin credenciales

No existe un endpoint oficial de OpenAI para inferencia sin API key. Sin credenciales, el programa cae automaticamente a `ReplayModelBackend` para que el experimento pueda ejecutarse sin configuracion adicional.