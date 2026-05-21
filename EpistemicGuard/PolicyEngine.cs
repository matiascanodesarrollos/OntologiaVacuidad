namespace EpistemicGuard;

public sealed class PolicyEngine
{
    public PolicyDecision Decide(ProjectionResult projection)
    {
        var confidence = Math.Clamp(
            projection.SupportScore * 0.6 + projection.CoverageScore * 0.3 - projection.ContradictionScore * 0.7,
            0.0,
            1.0);

        var evidenceIds = projection.TopEvidence.Select(x => x.Evidence.Id).ToList();

        if (projection.TopEvidence.Count == 0)
        {
            return new PolicyDecision(Verdict.Abstain, "Sin evidencia recuperada para sustentar la afirmacion.", 0.0, evidenceIds);
        }

        if (projection.ContradictionScore >= 0.2 && projection.ContradictionScore >= projection.SupportScore)
        {
            return new PolicyDecision(Verdict.Abstain, "Se detecto conflicto entre evidencia relevante.", confidence, evidenceIds);
        }

        if (projection.SupportScore >= 0.35 && projection.CoverageScore >= 0.35)
        {
            return new PolicyDecision(Verdict.Confirmed, "Evidencia consistente y cobertura suficiente.", confidence, evidenceIds);
        }

        if (projection.SupportScore >= 0.22 && projection.CoverageScore >= 0.25)
        {
            return new PolicyDecision(Verdict.Plausible, "Evidencia parcial; responder con cautela.", confidence, evidenceIds);
        }

        if (projection.SupportScore >= 0.12)
        {
            return new PolicyDecision(Verdict.Speculative, "Se observan indicios debiles; etiquetar como especulativo.", confidence, evidenceIds);
        }

        return new PolicyDecision(Verdict.Abstain, "Soporte insuficiente; mejor abstenerse o pedir mas contexto.", confidence, evidenceIds);
    }
}
