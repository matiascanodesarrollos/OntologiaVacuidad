# OntologiaVacuidad

Solución C# para Ontología Vacuidad:

Aplicación de consola y librerías para probar el sistema diseñado en el artículo de Medim titulado "Ontología de la vacuidad: un sistema para entender la realidad como modulación (AM-FM-PM) y plasma primordial (RGB)".

https://medium.com/@heroe.vajradharma/ontolog%C3%ADa-de-la-vacuidad-un-sistema-para-entender-la-realidad-como-modulaci%C3%B3n-am-fm-fsk-y-plasma-2c8b56f2d8ed

## Investigacion anti alucinaciones

Se agrego un proyecto separado EpistemicGuard para evaluar reduccion de alucinaciones mediante una canalizacion epistemica:

1. proyeccion de evidencia
2. scoring de soporte/contradiccion/cobertura
3. politica de respuesta con abstencion

### Hallazgos actuales

Benchmark comparativo medido (40 casos etiquetados):

1. Baseline: 50.00% de alucinacion total
2. Guarded: 0.00% de alucinacion total
3. Reduccion relativa observada: 100.00%

Conclusión: los resultados son prometedores para continuar la investigacion, aunque la configuracion actual del sistema guardado es demasiado conservadora (abstencion alta) y requiere calibracion para mejorar utilidad practica.

### Como verificar las conclusiones

1. Ejecutar benchmark:

	powershell
	dotnet run --project EpistemicGuard/EpistemicGuard.csproj -- --benchmark

2. Revisar salida de consola con:
	1. hallu_total
	2. hallu_assertive
	3. precision
	4. abstention

3. Revisar reporte persistido en:
	1. EpistemicGuard/benchmark-latest.md

### Como ampliar las pruebas

1. Editar EpistemicGuard/BenchmarkData.cs:
	1. agregar nuevos claims Supported/Contradicted/Unknown
	2. aumentar corpus con fuentes conflictivas y de distinta confiabilidad

2. Ajustar umbrales en EpistemicGuard/PolicyEngine.cs.
3. Repetir benchmark y comparar contra baseline para validar mejora real.

Para detalle tecnico y protocolo completo, ver EpistemicGuard/README.md.