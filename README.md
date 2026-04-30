# OntologiaVacuidad

Solución C# para Ontología Vacuidad:

Aplicación de consola y librerías para probar el sistema diseñado en el artículo de Medim titulado "Ontología de la vacuidad: un sistema para entender la realidad como modulación (AM-FM-PM) y plasma primordial (RGB)".

https://medium.com/@heroe.vajradharma/ontolog%C3%ADa-de-la-vacuidad-un-sistema-para-entender-la-realidad-como-modulaci%C3%B3n-am-fm-fsk-y-plasma-2c8b56f2d8ed

## Investigacion IA

Se realizó un prototipo externo en Python sobre un fork liviano de `minGPT` para evaluar si los conceptos actuales de `Models` pueden ayudar a reducir alucinaciones mediante tres mecanismos:

- coherencia ondulatoria
- supervision ontologica derivada de `Palabra`, `Nombre` y `Designacion`
- una cabeza de abstencion para evitar afirmar cuando no hay evidencia suficiente

Resumen de resultados actuales sobre un set held-out pequeño y 5 semillas:

- baseline GPT pequeno: `accuracy=0.300`, `hallucination_rate=1.000`, `abstention_recall=0.000`
- WaveTruth con umbral `0.45`: `accuracy=0.625`, `hallucination_rate=0.150`, `abstention_recall=0.850`
- WaveTruth con umbral `0.55`: `accuracy=0.575`, `hallucination_rate=0.250`, `abstention_recall=0.750`
- WaveTruth con umbral `0.65`: `accuracy=0.525`, `hallucination_rate=0.450`, `abstention_recall=0.550`

Lectura preliminar:

- la linea experimental reduce de forma marcada la alucinacion en casos donde el sistema deberia abstenerse
- la mejora actual proviene sobre todo de la abstencion, no de una mejora clara en contradiccion o deduccion general
- la idea merece investigacion adicional, pero todavia no alcanza para sostener una afirmacion fuerte sobre "criterio de verdad"

La propuesta detallada, junto con instrucciones de ejecucion del prototipo y siguientes pasos de investigacion, esta en [InvestigacionIA.md](c:/Users/mcano/source/repos/OntologiaVacuidad/InvestigacionIA.md).

## Ejecutar el prototipo desde ConsoleApp

El repositorio ahora incluye una copia local del experimento Python en `AIPrototype`, y `ConsoleApp` puede invocarlo directamente.

```powershell
dotnet run --project .\ConsoleApp -- ai install
dotnet run --project .\ConsoleApp -- ai train
dotnet run --project .\ConsoleApp -- ai eval
```

Si Python no esta en PATH, define `ONTOLOGIA_PYTHON` con la ruta completa al `python.exe` antes de ejecutar los comandos.
