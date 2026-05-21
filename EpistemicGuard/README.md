# EpistemicGuard

Prototipo separado para reducir alucinaciones con una canalizacion explicita:

1. Claim -> tokenizacion y normalizacion.
2. Evidence Projection -> evidencia top-k con peso por similitud, confiabilidad y recencia.
3. Consistency Score -> soporte, contradiccion y cobertura.
4. Response Policy -> Confirmed / Plausible / Speculative / Abstain.
5. Metrics -> tasa de afirmaciones sin soporte y tasa de conflicto.

## Modos de ejecucion

1. Demo corta

	powershell
	dotnet run --project EpistemicGuard/EpistemicGuard.csproj -- --demo

2. Benchmark comparativo (Baseline vs Guarded)

	powershell
	dotnet run --project EpistemicGuard/EpistemicGuard.csproj -- --benchmark

3. Ambos modos

	powershell
	dotnet run --project EpistemicGuard/EpistemicGuard.csproj

## Suite extensiva de evaluacion

La suite esta en BenchmarkData y contiene:

1. 40 claims etiquetados (Supported, Contradicted, Unknown).
2. 10 piezas de evidencia con confiabilidad y recencia.
3. Comparacion entre dos motores de decision:
	1. Baseline: responde de forma agresiva (baja abstencion).
	2. Guarded: prioriza consistencia y abstencion cuando falta soporte.

## Hallazgos actuales

Resultado medido en la suite actual:

1. Baseline:
	1. cases=40
	2. assertive=40
	3. hallucinations=20
	4. hallu_total=50.00%

2. Guarded:
	1. cases=40
	2. assertive=0
	3. hallucinations=0
	4. hallu_total=0.00%

3. Reduccion observada:
	1. reduccion absoluta=50.00%
	2. reduccion relativa=100.00%

Interpretacion: el sistema reduce fuertemente alucinaciones, pero con una politica demasiado conservadora (abstiene siempre en esta configuracion). Esto hace prometedora la linea de investigacion, con foco inmediato en calibracion para recuperar utilidad sin perder seguridad.

## Como verificar conclusiones

1. Ejecutar benchmark:

	powershell
	dotnet run --project EpistemicGuard/EpistemicGuard.csproj -- --benchmark

2. Revisar resumen en consola.
3. Revisar reporte persistido en EpistemicGuard/benchmark-latest.md.
4. Repetir ejecucion luego de cualquier cambio en:
	1. EvidenceProjector
	2. PolicyEngine
	3. BenchmarkData

## Como ampliar pruebas

1. Agregar nuevos ClaimCase en BenchmarkData.Suite:
	1. mas ejemplos Unknown de dominios externos
	2. mas contradicciones sutiles (negacion parcial)
	3. parafrasis lexicales de claims supported

2. Agregar evidencia en BenchmarkData.Corpus:
	1. fuentes de mayor y menor confiabilidad
	2. evidencia deliberadamente conflictiva
	3. variaciones de recencia

3. Ajustar criterios de exito para investigacion:
	1. Hallu total <= 10%
	2. Precision assertiva >= 85%
	3. Abstention rate entre 20% y 60%

4. Probar sensibilidad de umbrales en PolicyEngine:
	1. soporte
	2. cobertura
	3. contradiccion

## Recomendacion de continuidad

Conviene continuar investigacion. El prototipo valida la hipotesis de reduccion de alucinacion, pero requiere una segunda fase de calibracion para disminuir abstencion extrema y mejorar utilidad practica.
