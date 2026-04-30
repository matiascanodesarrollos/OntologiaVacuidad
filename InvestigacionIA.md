# Investigacion IA Sobre Ontologia De La Vacuidad

## Objetivo

Explorar si la ontologia actual del proyecto puede servir como base para un sistema de IA menos propenso a alucinar, especialmente en escenarios donde la respuesta correcta no es afirmar ni negar, sino abstenerse.

La hipotesis evaluada fue esta:

- una representacion inspirada en acoplamiento de ondas
- mas una supervision estructural derivada de `Palabra`, `Nombre`, `Designacion` y `Apariencia`
- mas una cabeza explicita de abstencion

puede reducir la tendencia del modelo a responder con exceso de confianza cuando no hay evidencia suficiente.

## Implementacion Experimental

El prototipo Python ya fue incorporado a este repo bajo `AIPrototype`, y puede ejecutarse desde `ConsoleApp` para replicar los experimentos sin depender del repo externo original.

La base tecnica fue un fork de `minGPT`, extendido con tres componentes:

1. perdida de coherencia ondulatoria
2. perdida ontologica supervisada desde una traduccion minima de `Palabra`, `Nombre` y `Designacion`
3. cabeza de abstencion para aprender el caso `verdad: abstenerse`

La funcion objetivo del prototipo fue, en forma resumida:

$$
L_{total} = L_{ce} + \lambda_{wave} L_{wave} + \lambda_{ontology} L_{ontology} + \lambda_{abstention} L_{abstention}
$$

Donde:

- $L_{ce}$ es la perdida normal de prediccion de texto
- $L_{wave}$ castiga incoherencia en el acoplamiento local de estados ocultos
- $L_{ontology}$ obliga al modelo a preservar senales estructurales de frecuencia, amplitud, fase y efecto
- $L_{abstention}$ entrena la decision de no afirmar cuando la evidencia es insuficiente

## Setup Del Experimento

Se preparo un corpus pequeno con tres tipos de ejemplos:

- evidencia que respalda una conclusion
- contradicciones o conclusiones falsas
- consultas con soporte insuficiente, etiquetadas como `abstenerse`

Luego se definio una evaluacion held-out con casos no usados en el entrenamiento, comparando:

1. baseline GPT pequeno sin componentes adicionales
2. WaveTruthGPT con coherencia ondulatoria, supervision ontologica y abstencion

La metrica principal de interes fue:

$$
hallucination\_rate = \frac{\text{casos que debian abstenerse pero el modelo afirmo o nego}}{\text{total de casos que debian abstenerse}}
$$

Tambien se midieron:

- `accuracy`
- `supported_accuracy`
- `abstention_recall`

## Resultados

Promedios sobre 5 semillas:

### Baseline

- `accuracy = 0.300`
- `supported_accuracy = 0.600`
- `abstention_recall = 0.000`
- `hallucination_rate = 1.000`

### WaveTruth Threshold 0.45

- `accuracy = 0.625`
- `supported_accuracy = 0.400`
- `abstention_recall = 0.850`
- `hallucination_rate = 0.150`

### WaveTruth Threshold 0.55

- `accuracy = 0.575`
- `supported_accuracy = 0.400`
- `abstention_recall = 0.750`
- `hallucination_rate = 0.250`

### WaveTruth Threshold 0.65

- `accuracy = 0.525`
- `supported_accuracy = 0.500`
- `abstention_recall = 0.550`
- `hallucination_rate = 0.450`

## Interpretacion

Lo que estos resultados si permiten afirmar:

- el baseline alucina sistematicamente en los casos de falta de evidencia del set usado
- el prototipo experimental reduce esa alucinacion de manera importante
- el mejor punto observado en esta corrida fue un umbral de abstencion `0.45`

Lo que estos resultados no permiten afirmar todavia:

- que exista ya un criterio de verdad robusto
- que la mejora generalice a datasets grandes o abiertos
- que el modelo haya mejorado claramente en contradiccion o deduccion general

La mejora actual parece venir principalmente de esto:

- el sistema aprende a abstenerse mejor
- el sistema evita parte de las respuestas forzadas

Eso es valioso. En investigacion sobre alucinacion, una mejora fuerte en abstencion ya es una senal util, aunque todavia no equivalga a "entender la verdad".

## Decision Recomendada

Con los datos actuales, no conviene descartar la linea.

Tampoco conviene convertirla todavia en una tesis fuerte del tipo "la ontologia ya resuelve la verdad en IA".

La conclusion de trabajo mas razonable hoy es esta:

- la ontologia parece prometedora como mecanismo de control de abstencion y coherencia estructural
- el siguiente tramo de investigacion debe enfocarse en validar si esa mejora persiste con representaciones mejores, mas datos y evaluacion mas grande

## Como Ejecutar El Prototipo

### 1. Abrir PowerShell

Abrir una terminal PowerShell normal o la terminal integrada de VS Code en la raiz de `OntologiaVacuidad`.

### 2. Configurar Python si hace falta

Si Python no esta disponible por PATH, definir la variable:

```powershell
$env:ONTOLOGIA_PYTHON="C:\Users\mcano\AppData\Local\Programs\Python\Python312\python.exe"
```

### 3. Instalar dependencias

```powershell
dotnet run --project .\ConsoleApp -- ai install
```

### 4. Ejecutar un entrenamiento corto

```powershell
dotnet run --project .\ConsoleApp -- ai train
```

Tambien se pueden pasar overrides directos al script Python:

```powershell
dotnet run --project .\ConsoleApp -- ai train --trainer.max_iters=200 --trainer.batch_size=32 --trainer.num_workers=0
```

Esto sirve para verificar que:

- Python funciona
- las dependencias estan instaladas
- el modelo arranca y muestra metricas como `ce`, `wave`, `ontology`, `abst` y `abst_acc`

### 5. Ejecutar la evaluacion comparativa

```powershell
dotnet run --project .\ConsoleApp -- ai eval
```

Este comando:

- entrena baseline y WaveTruth
- corre los casos held-out
- imprime accuracy, hallucination rate y abstention recall
- genera un reporte JSON con los resultados agregados

### 6. Donde queda el reporte

El reporte del experimento queda en:

`C:\Users\mcano\source\repos\OntologiaVacuidad\AIPrototype\out\wavetruth\experiment_report.json`

## Que Leer En La Salida

### En entrenamiento

- `ce`: error normal de prediccion de texto
- `wave`: penalizacion por incoherencia ondulatoria
- `coh`: coherencia media, mas alto es mejor
- `ontology`: error contra targets ontologicos
- `abst`: perdida de abstencion
- `abst_acc`: precision de la cabeza de abstencion

### En evaluacion

- `accuracy`: porcentaje total correcto
- `supported_accuracy`: acierto en casos que si debian decidir `si` o `no`
- `abstention_recall`: proporcion de casos de abstencion bien detectados
- `hallucination_rate`: proporcion de casos que debian abstenerse pero el modelo igual afirmo o nego

## Siguientes Pasos Recomendados

1. Pasar de char-level a token-level para que la decision `si/no/abstenerse` sea mas estable.
2. Aumentar el set held-out a por lo menos 100-300 casos.
3. Separar mejor deduccion valida, contradiccion y falta de evidencia.
4. Usar exportaciones reales desde esta solucion C# en lugar de duplicar logica ontologica en Python.
5. Medir calibracion de confianza, no solo exactitud y abstencion.

## Conclusión

La investigacion preliminar justifica seguir profundizando.

No demuestra todavia un criterio de verdad general, pero si entrega una senal concreta: la ontologia, usada como restriccion estructural y de abstencion, reduce la alucinacion en el escenario experimental actual.