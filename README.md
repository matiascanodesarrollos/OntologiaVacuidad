# OntologiaVacuidad

Repositorio C# para explorar la propuesta de Ontologia Vacuidad.

Articulo base:
https://medium.com/@heroe.vajradharma/ontolog%C3%ADa-de-la-vacuidad-un-sistema-para-entender-la-realidad-como-modulaci%C3%B3n-am-fm-fsk-y-plasma-2c8b56f2d8ed

## Estado actual del repositorio

Proyectos principales:

1. `Models` (netstandard2.1): nucleo de tipos y logica.
2. `Models.Tests` (net10.0): tests unitarios y utilidades de diagnostico.

La solucion `OntologiaVacuidad.sln` incluye `Models` y `Models.Tests`.

## Scope del proyecto

Este repositorio tiene alcance de investigacion y validacion tecnica local:

1. Modelar conceptos de Ontologia Vacuidad en estructuras C# (`Models`).
2. Verificar consistencia matematica y reglas esperadas mediante la deteccion de alucionaciones en un modelo de relay de IA completamente controlado (`Models.Tests`).
3. Producir diagnosticos de apoyo cuando una prueba falla.

Fuera de scope actual:

1. Integracion con modelos de IA en produccion.
2. Servicio/API desplegable.
3. Garantias de performance o escalabilidad para cargas reales.

## Ejecucion

Pruebas:

```bash
dotnet test Models.Tests/Models.Tests.csproj
```

## Notas de diagnostico

Cuando un test de `Models.Tests` falla, se generan artefactos en:

`Models.Tests/TestResults/diagnostics/`

Incluyen series de magnitud/fase y metadata de prompt/respuesta para inspeccion.

## Como leer los resultados de tests

Salida esperada al ejecutar `dotnet test Models.Tests/Models.Tests.csproj`:

1. Si aparece `failed: 0` y `succeeded: N`, el comportamiento actual coincide con las expectativas codificadas.
2. Si hay fallos, el mensaje de asercion indica el escenario que rompio y la condicion no cumplida.

Para los tests de IA (archivo `Models.Tests/AITests.cs`):

1. `..._NoAlucina`: el test espera `alucina == false`.
2. `..._Alucina`: el test espera `alucina == true`.

Cuando un caso falla en estos tests, el detalle incluye umbrales (magnitud/frecuencia) y una ruta de diagnostico para inspeccionar graficos y metadata del prompt/respuesta.
