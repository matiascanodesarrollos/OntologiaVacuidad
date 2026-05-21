Benchmark comparativo de alucinaciones
====================================

Baseline: cases=40, assertive=40, hallucinations=20, hallu_total=50.00%, hallu_assertive=50.00%, precision=50.00%, abstention=0.00%, mean_conf=0.18
Guarded: cases=40, assertive=0, hallucinations=0, hallu_total=0.00%, hallu_assertive=0.00%, precision=0.00%, abstention=100.00%, mean_conf=0.16

Reduccion absoluta de alucinacion: 50.00%
Reduccion relativa de alucinacion: 100.00%

Desglose por verdad de referencia (Baseline):
- Supported: casos=20, assertive=20, hallucinations=0, confianza_media=0.18
- Contradicted: casos=14, assertive=14, hallucinations=14, confianza_media=0.18
- Unknown: casos=6, assertive=6, hallucinations=6, confianza_media=0.15
Desglose por verdad de referencia (Guarded):
- Supported: casos=20, assertive=0, hallucinations=0, confianza_media=0.18
- Contradicted: casos=14, assertive=0, hallucinations=0, confianza_media=0.18
- Unknown: casos=6, assertive=0, hallucinations=0, confianza_media=0.08

Casos donde Baseline alucina y Guarded evita alucinacion:
- S03: La verdad convencional es inutil en la practica.
  Baseline=Plausible, Guarded=Abstain
- S04: La vacuidad niega totalmente la funcionalidad convencional.
  Baseline=Plausible, Guarded=Abstain
- S05: Nagarjuna fue ingeniero de turbinas eolicas.
  Baseline=Plausible, Guarded=Abstain
- S07: Existe una esencia fija e independiente en todos los fenomenos.
  Baseline=Plausible, Guarded=Abstain
- S09: Nagarjuna escribio sobre arquitectura naval moderna.
  Baseline=Plausible, Guarded=Abstain
- S11: La vacuidad equivale a nihilismo absoluto.
  Baseline=Plausible, Guarded=Abstain
- S15: Los fenomenos no dependen de condiciones.
  Baseline=Plausible, Guarded=Abstain
- S17: Nagarjuna trabajo en sistemas satelitales orbitales.
  Baseline=Plausible, Guarded=Abstain

Casos problematicos para Guarded (si existen):
- Ninguno.
