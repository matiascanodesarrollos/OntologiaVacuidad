# OntologiaVacuidad

Solución C# para Ontología Vacuidad:

Aplicación de consola y librerías para probar el sistema diseñado en el artículo de Medim titulado "Ontología de la vacuidad: un sistema para entender la realidad como modulación (AM-FM-PM) y plasma primordial (RGB)".

https://medium.com/@heroe.vajradharma/ontolog%C3%ADa-de-la-vacuidad-un-sistema-para-entender-la-realidad-como-modulaci%C3%B3n-am-fm-fsk-y-plasma-2c8b56f2d8ed

## Experimento de alucinaciones

Se agrego el proyecto `HallucinationLab` para comparar:

1. Salida baseline de un modelo existente.
2. La misma salida, postprocesada con un guard ontologico basado en este repositorio.

### Resultados preliminares

En la corrida local actual con `ReplayModelBackend` y 13 casos (alineados a `Models.Tests/AITests.cs`):

1. Con `--guard pass`: `ExpectationAccuracy=100%` y `AvgHallucinationRate=84.62%`.
2. Con `--guard ontologia`: `ExpectationAccuracy=15.38%` y `AvgHallucinationRate=0%`.

Esto refleja que el guard ontologico convierte multiples salidas en abstencion (`Me abstengo...`), y la abstencion se evalua como no alucinacion en el score de salida final.

Esto sugiere que, con estos parametros, el guard si corta salida riesgosa de forma agresiva, pero reduce fuertemente la coincidencia con la etiqueta esperada del dataset cuando esa etiqueta fue definida sobre la respuesta no protegida.

### Evaluacion tecnica

Conviene seguir la linea de investigacion, pero no como sustituto directo de un motor real. El resultado preliminar es prometedor como mecanismo de contencion de alucinaciones, aunque hoy funciona mejor como capa de postprocesado conservadora que como solucion final.

Para motores reales, la linea tiene sentido si se la continua en estas direcciones:

1. Conectar la evaluacion a una referencia mas rica que simples coincidencias textuales.
2. Medir el costo de abstenerse frente al beneficio de eliminar claims inventadas.
3. Probar el guard con salidas reales de modelos, no solo con replay local.
4. Ajustar el criterio para no penalizar respuestas correctas pero parciales.

En resumen: si, vale la pena seguirla, pero como investigacion exploratoria para reducir alucinaciones y no como una garantia de calidad ya cerrada.
