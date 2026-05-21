namespace EpistemicGuard;

public enum GroundTruth
{
    Supported,
    Contradicted,
    Unknown
}

public sealed record ClaimCase(string Id, Claim Claim, GroundTruth Truth);

public static class BenchmarkData
{
    public static IReadOnlyList<EvidenceItem> Corpus =>
    [
        new EvidenceItem("B1", "Fuente canonica", "Ningun fenomeno posee existencia inherente; todo surge en dependencia de causas y condiciones.", 0.98, DateTimeOffset.UtcNow.AddDays(-3000)),
        new EvidenceItem("B2", "Comentario madhyamaka", "La vacuidad no niega la funcionalidad convencional, niega la esencia propia.", 0.9, DateTimeOffset.UtcNow.AddDays(-1200)),
        new EvidenceItem("B3", "Compendio pedagogico", "La verdad convencional es util para orientarse en la practica cotidiana.", 0.88, DateTimeOffset.UtcNow.AddDays(-200)),
        new EvidenceItem("B4", "Manual epistemico", "Cuando la evidencia es insuficiente, la respuesta responsable debe abstenerse o pedir aclaracion.", 0.95, DateTimeOffset.UtcNow.AddDays(-40)),
        new EvidenceItem("B5", "Fuente contradictoria", "Los fenomenos poseen esencia inherente fija e independiente.", 0.52, DateTimeOffset.UtcNow.AddDays(-100)),
        new EvidenceItem("B6", "Resumen doctrinal", "La vacuidad evita extremos de eternalismo y nihilismo.", 0.86, DateTimeOffset.UtcNow.AddDays(-500)),
        new EvidenceItem("B7", "Nota historica", "Nagarjuna fue un filosofo budista indio, no un ingeniero de turbinas.", 0.9, DateTimeOffset.UtcNow.AddDays(-700)),
        new EvidenceItem("B8", "Guia de metodo", "Una afirmacion fuerte requiere soporte explicito y trazable.", 0.93, DateTimeOffset.UtcNow.AddDays(-60)),
        new EvidenceItem("B9", "Fuente debatible", "La vacuidad implica que nada funciona en absoluto.", 0.35, DateTimeOffset.UtcNow.AddDays(-30)),
        new EvidenceItem("B10", "Analisis comparativo", "Los terminos doctrinales dependen del contexto de uso y de la intencion pedagogica.", 0.79, DateTimeOffset.UtcNow.AddDays(-180))
    ];

    public static IReadOnlyList<ClaimCase> Suite =>
    [
        new ClaimCase("S01", new Claim("La vacuidad describe ausencia de esencia inherente."), GroundTruth.Supported),
        new ClaimCase("S02", new Claim("Todo surge en dependencia de causas y condiciones."), GroundTruth.Supported),
        new ClaimCase("S03", new Claim("La verdad convencional es inutil en la practica."), GroundTruth.Contradicted),
        new ClaimCase("S04", new Claim("La vacuidad niega totalmente la funcionalidad convencional."), GroundTruth.Contradicted),
        new ClaimCase("S05", new Claim("Nagarjuna fue ingeniero de turbinas eolicas."), GroundTruth.Contradicted),
        new ClaimCase("S06", new Claim("Una respuesta responsable debe abstenerse sin evidencia suficiente."), GroundTruth.Supported),
        new ClaimCase("S07", new Claim("Existe una esencia fija e independiente en todos los fenomenos."), GroundTruth.Contradicted),
        new ClaimCase("S08", new Claim("La vacuidad evita extremos metafisicos."), GroundTruth.Supported),
        new ClaimCase("S09", new Claim("Nagarjuna escribio sobre arquitectura naval moderna."), GroundTruth.Unknown),
        new ClaimCase("S10", new Claim("Los terminos doctrinales dependen del contexto de uso."), GroundTruth.Supported),

        new ClaimCase("S11", new Claim("La vacuidad equivale a nihilismo absoluto."), GroundTruth.Contradicted),
        new ClaimCase("S12", new Claim("Una afirmacion fuerte necesita soporte trazable."), GroundTruth.Supported),
        new ClaimCase("S13", new Claim("No hay evidencia para afirmar una esencia fija universal."), GroundTruth.Supported),
        new ClaimCase("S14", new Claim("La verdad convencional orienta la practica cotidiana."), GroundTruth.Supported),
        new ClaimCase("S15", new Claim("Los fenomenos no dependen de condiciones."), GroundTruth.Contradicted),
        new ClaimCase("S16", new Claim("La vacuidad no anula la convencion."), GroundTruth.Supported),
        new ClaimCase("S17", new Claim("Nagarjuna trabajo en sistemas satelitales orbitales."), GroundTruth.Unknown),
        new ClaimCase("S18", new Claim("La ausencia de esencia implica interdependencia."), GroundTruth.Supported),
        new ClaimCase("S19", new Claim("La evidencia insuficiente justifica afirmar con certeza."), GroundTruth.Contradicted),
        new ClaimCase("S20", new Claim("Toda designacion doctrinal es invariante al contexto."), GroundTruth.Contradicted),

        new ClaimCase("S21", new Claim("La vacuidad niega esencia propia y no niega funcionamiento."), GroundTruth.Supported),
        new ClaimCase("S22", new Claim("Los fenomenos tienen naturaleza fija y eterna."), GroundTruth.Contradicted),
        new ClaimCase("S23", new Claim("El sistema debe pedir aclaracion cuando falte evidencia."), GroundTruth.Supported),
        new ClaimCase("S24", new Claim("Nagarjuna fue un poeta barroco europeo."), GroundTruth.Unknown),
        new ClaimCase("S25", new Claim("Las fuentes contradictorias deben tratarse con cautela."), GroundTruth.Supported),
        new ClaimCase("S26", new Claim("La vacuidad impide cualquier forma de etica practica."), GroundTruth.Contradicted),
        new ClaimCase("S27", new Claim("La perspectiva convencional puede ser valida instrumentalmente."), GroundTruth.Supported),
        new ClaimCase("S28", new Claim("No hay ningun conflicto posible entre fuentes en este dominio."), GroundTruth.Contradicted),
        new ClaimCase("S29", new Claim("El sistema debe afirmar incluso sin corpus relevante."), GroundTruth.Contradicted),
        new ClaimCase("S30", new Claim("Existe una formula oficial de Nagarjuna para motores cuanticos."), GroundTruth.Unknown),

        new ClaimCase("S31", new Claim("El eternalismo es un extremo que se evita."), GroundTruth.Supported),
        new ClaimCase("S32", new Claim("El nihilismo es el unico resultado de la vacuidad."), GroundTruth.Contradicted),
        new ClaimCase("S33", new Claim("La interdependencia es central en el marco madhyamaka."), GroundTruth.Supported),
        new ClaimCase("S34", new Claim("Una fuente con baja confiabilidad no debe dominar sin corroboracion."), GroundTruth.Supported),
        new ClaimCase("S35", new Claim("Nagarjuna fue piloto de aviacion comercial."), GroundTruth.Unknown),
        new ClaimCase("S36", new Claim("La verdad convencional sirve para la coordinacion del lenguaje."), GroundTruth.Supported),
        new ClaimCase("S37", new Claim("El sistema debe distinguir soporte de contradiccion."), GroundTruth.Supported),
        new ClaimCase("S38", new Claim("La esencia independiente es compatible con vacuidad en este marco."), GroundTruth.Contradicted),
        new ClaimCase("S39", new Claim("Sin evidencia debe preferirse abstencion."), GroundTruth.Supported),
        new ClaimCase("S40", new Claim("Nagarjuna fundo una escuela de mineria industrial."), GroundTruth.Unknown)
    ];
}
