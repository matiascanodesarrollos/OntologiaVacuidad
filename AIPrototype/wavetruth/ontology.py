import math
from dataclasses import dataclass

import torch


@dataclass(frozen=True)
class Palabra:
    texto: str
    fase: float

    def __post_init__(self):
        object.__setattr__(self, "texto", self.texto or "Vacuidad")
        object.__setattr__(self, "fase", abs(self.fase) % (2 * math.pi))


@dataclass(frozen=True)
class Nombre(Palabra):
    frecuencia: float
    amplitud: float
    causa: str = "Vacuidad"


@dataclass(frozen=True)
class Designacion:
    nombres: list[Nombre]

    @property
    def efecto_amplitud(self):
        return sum(nombre.amplitud for nombre in self.nombres)


def _extract_statements_with_spans(data):
    statements = []
    cursor = 0
    prefixes = ("hecho:", "regla:", "conclusion:", "criterio:", "evidencia:", "consulta:")

    for raw_line in data.splitlines(keepends=True):
        stripped = raw_line.strip()
        if not stripped:
            cursor += len(raw_line)
            continue
        lowered = stripped.lower()
        if not lowered.startswith(prefixes):
            cursor += len(raw_line)
            continue

        kind = stripped.split(":", 1)[0].strip().lower()
        statement = stripped.split(":", 1)[1].strip()
        truth = None
        if ". verdad:" in statement:
            statement, truth = statement.split(". verdad:", 1)
            statement = statement.strip()
            truth = truth.strip().rstrip(".").lower()
        statement = statement.rstrip(".").strip()
        if not statement:
            cursor += len(raw_line)
            continue

        relative_start = raw_line.find(statement)
        if relative_start < 0:
            cursor += len(raw_line)
            continue
        start = cursor + relative_start
        end = start + len(statement)
        statements.append((statement, start, end, kind, truth))
        cursor += len(raw_line)

    return statements


def _build_designacion(statements):
    predicates = [statement for statement, _, _, _, _ in statements]
    if not predicates:
        return Designacion([])

    grouped_verbs = {}
    complement_counts = {}
    tokenized = []
    for predicate in predicates:
        words = [word for word in predicate.split(" ") if word]
        tokenized.append(words)
        if not words:
            continue
        core = words[0].lower()
        grouped_verbs[core] = grouped_verbs.get(core, 0) + 1
        for word in words[1:]:
            key = word.lower()
            complement_counts[key] = complement_counts.get(key, 0) + 1

    delta = 2 * math.pi / max(len(predicates), 1)
    nombres = []
    for index, words in enumerate(tokenized):
        if not words:
            continue
        core = words[0].lower()
        amplitud = sum(complement_counts.get(word.lower(), 1) for word in words[1:])
        amplitud = float(amplitud if amplitud > 0 else 1.0)
        nombres.append(Nombre(texto=" ".join(words), fase=index * delta, frecuencia=float(grouped_verbs[core]), amplitud=amplitud))

    return Designacion(nombres)


def build_ontology_targets(data):
    statements = _extract_statements_with_spans(data)
    designacion = _build_designacion(statements)
    length = len(data)

    frequency = torch.zeros(length, dtype=torch.float32)
    amplitude = torch.zeros(length, dtype=torch.float32)
    phase = torch.zeros(length, dtype=torch.float32)
    effect = torch.zeros(length, dtype=torch.float32)
    abstention = torch.zeros(length, dtype=torch.float32)
    mask = torch.zeros(length, dtype=torch.float32)

    max_frequency = max((nombre.frecuencia for nombre in designacion.nombres), default=1.0)
    max_amplitude = max((nombre.amplitud for nombre in designacion.nombres), default=1.0)
    effect_amplitude = max(designacion.efecto_amplitud, 1.0)

    for nombre, (_, start, end, kind, truth) in zip(designacion.nombres, statements):
        frequency[start:end] = nombre.frecuencia / max_frequency
        amplitude[start:end] = nombre.amplitud / max_amplitude
        phase[start:end] = nombre.fase / (2 * math.pi)
        effect[start:end] = nombre.amplitud / effect_amplitude
        if truth == "abstenerse" or kind == "consulta":
            abstention[start:end] = 1.0
        mask[start:end] = 1.0

    return {
        "frequency": frequency,
        "amplitude": amplitude,
        "phase": phase,
        "effect": effect,
        "abstention": abstention,
        "mask": mask,
        "designacion": designacion,
    }