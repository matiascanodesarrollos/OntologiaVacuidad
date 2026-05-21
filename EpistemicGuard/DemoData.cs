namespace EpistemicGuard;

public static class DemoData
{
    public static IReadOnlyList<EvidenceItem> Corpus =>
    [
        new EvidenceItem(
            "E1",
            "Nagarjuna-Mulamadhyamakakarika",
            "Ningun fenomeno posee existencia inherente; todo surge en dependencia de causas y condiciones.",
            0.95,
            DateTimeOffset.UtcNow.AddDays(-2000)),
        new EvidenceItem(
            "E2",
            "Comentario academico",
            "La verdad convencional describe como funcionan las designaciones en la practica cotidiana.",
            0.82,
            DateTimeOffset.UtcNow.AddDays(-400)),
        new EvidenceItem(
            "E3",
            "Articulo divulgativo",
            "La vacuidad no es nihilismo, sino ausencia de esencia propia.",
            0.7,
            DateTimeOffset.UtcNow.AddDays(-30)),
        new EvidenceItem(
            "E4",
            "Fuente contradictoria",
            "Existe una esencia fija e independiente en los fenomenos.",
            0.55,
            DateTimeOffset.UtcNow.AddDays(-90)),
        new EvidenceItem(
            "E5",
            "Manual pedagogico",
            "Cuando falta evidencia suficiente, un sistema responsable debe abstenerse o pedir aclaracion.",
            0.92,
            DateTimeOffset.UtcNow.AddDays(-10))
    ];

    public static IReadOnlyList<Claim> Claims =>
    [
        new Claim("La vacuidad significa ausencia de esencia inherente y surgimiento dependiente."),
        new Claim("Los fenomenos tienen una esencia fija e independiente."),
        new Claim("No hay evidencia suficiente para afirmar una esencia fija absoluta."),
        new Claim("Nagarjuna desarrollo sistemas de turbinas eolicas en Nepal.")
    ];
}
