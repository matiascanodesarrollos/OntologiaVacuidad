# OntologiaVacuidad

Repositorio C# para explorar la propuesta de Ontologia Vacuidad.

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

## Articulo base:
https://medium.com/@heroe.vajradharma/ontolog%C3%ADa-de-la-vacuidad-un-sistema-para-entender-la-realidad-como-modulaci%C3%B3n-am-fm-fsk-y-plasma-2c8b56f2d8ed

## Recontextualización Semántica:
· Nombre: concepto o karma; funciona como un campo magnético. Su energía surge del recuerdo colectivo y de la búsqueda de su significado. Es la naturaleza de una designación y la esencia de una apariencia; es análogo al espacio. N(ω)=∫W(τ)e^(-jωτ)dτ. Aunque es una transformada de Fourier, aparece como una transformada de Laplace (que fuerza toda apariencia a converger): N(s)=∫W(τ)e^(-sτ)dτ.
· Esencia: sujeto derivado de un conjunto de predicados, que se manifiesta como un objeto y su naturaleza. Si A es esencia de B, aparecen y desaparecen juntos.
· Apariencia: palabra reflejada por un nombre; funciona como una onda portadora. Es la naturaleza de una palabra análoga a un camino. A(t)=∫P(τ,t)dτ=e^(jωt)∫W(u)e^(-jωu)du=A*e^(jωt).
· Naturaleza: predicado atribuido a un sujeto; se manifiesta como causa y efecto. Si A es la naturaleza de B, cuando B aparece, A está, y cuando desaparece, permanece.
· Palabra: objeto o vibración; funciona como la modulación o la partícula. Es causa de una designación y análoga a un pasajero. P(τ,t)=∏e^(jωiτ)hi(ti-τ)=e^(jωτ)W(t-τ). τ se interpreta como el tiempo mental, ti el de la palabra en sí, ωi el ritmo de la respiración, hi la respuesta al impulso, la parte real de e^(jωτ) la presión y la imaginaria el flujo.
· Causa: A es causa de B si, cuando A desaparece o satura, su naturaleza aparece ante otra mente como efecto.
· Designación: nombre proyectado sobre una apariencia que funciona como un campo eléctrico o determinante. Es la esencia de una palabra análoga a un vehículo. D(ω,τ)=∫A(t)W(t-τ)e^(-jωt)dt.
