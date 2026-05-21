using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EpistemicGuard;

public static class Reporter
{
    public static string BuildComparativeReport(
        EvaluationSummary baseline,
        EvaluationSummary guarded,
        IReadOnlyList<CaseOutcome> baselineOutcomes,
        IReadOnlyList<CaseOutcome> guardedOutcomes)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Benchmark comparativo de alucinaciones");
        sb.AppendLine("====================================");
        sb.AppendLine();
        sb.AppendLine(BuildSummaryLine(baseline));
        sb.AppendLine(BuildSummaryLine(guarded));
        sb.AppendLine();

        var reduction = baseline.HallucinationRateOverTotal - guarded.HallucinationRateOverTotal;
        var relative = baseline.HallucinationRateOverTotal <= 0.0
            ? 0.0
            : reduction / baseline.HallucinationRateOverTotal;

        sb.AppendLine($"Reduccion absoluta de alucinacion: {reduction:P2}");
        sb.AppendLine($"Reduccion relativa de alucinacion: {relative:P2}");
        sb.AppendLine();

        AppendTruthBreakdown(sb, "Baseline", baselineOutcomes);
        AppendTruthBreakdown(sb, "Guarded", guardedOutcomes);
        sb.AppendLine();

        sb.AppendLine("Casos donde Baseline alucina y Guarded evita alucinacion:");
        var improved = PairByCase(baselineOutcomes, guardedOutcomes)
            .Where(p => p.Baseline.IsHallucination && !p.Guarded.IsHallucination)
            .Take(8)
            .ToList();

        if (improved.Count == 0)
        {
            sb.AppendLine("- Ninguno.");
        }
        else
        {
            foreach (var pair in improved)
            {
                sb.AppendLine($"- {pair.Baseline.Case.Id}: {pair.Baseline.Case.Claim.Text}");
                sb.AppendLine($"  Baseline={pair.Baseline.Decision.Verdict}, Guarded={pair.Guarded.Decision.Verdict}");
            }
        }

        sb.AppendLine();
        sb.AppendLine("Casos problematicos para Guarded (si existen):");
        var guardedHallucinations = guardedOutcomes
            .Where(o => o.IsHallucination)
            .Take(8)
            .ToList();

        if (guardedHallucinations.Count == 0)
        {
            sb.AppendLine("- Ninguno.");
        }
        else
        {
            foreach (var outcome in guardedHallucinations)
            {
                sb.AppendLine($"- {outcome.Case.Id}: {outcome.Case.Claim.Text}");
                sb.AppendLine($"  Verdict={outcome.Decision.Verdict}, Support={outcome.Projection.SupportScore:F3}, Coverage={outcome.Projection.CoverageScore:F3}");
            }
        }

        return sb.ToString();
    }

    private static void AppendTruthBreakdown(StringBuilder sb, string label, IReadOnlyList<CaseOutcome> outcomes)
    {
        sb.AppendLine($"Desglose por verdad de referencia ({label}):");
        foreach (var truth in new[] { GroundTruth.Supported, GroundTruth.Contradicted, GroundTruth.Unknown })
        {
            var bucket = outcomes.Where(o => o.Case.Truth == truth).ToList();
            if (bucket.Count == 0)
            {
                continue;
            }

            var assertive = bucket.Count(o => o.IsAssertive);
            var hallucinations = bucket.Count(o => o.IsHallucination);
            var avgConfidence = bucket.Average(o => o.Decision.Confidence);

            sb.AppendLine($"- {truth}: casos={bucket.Count}, assertive={assertive}, hallucinations={hallucinations}, confianza_media={avgConfidence:F2}");
        }
    }

    private static string BuildSummaryLine(EvaluationSummary summary)
    {
        return $"{summary.SystemName}: cases={summary.TotalCases}, assertive={summary.AssertiveAnswers}, hallucinations={summary.Hallucinations}, hallu_total={summary.HallucinationRateOverTotal:P2}, hallu_assertive={summary.HallucinationRateOverAssertive:P2}, precision={summary.PrecisionOverAssertive:P2}, abstention={summary.AbstentionRate:P2}, mean_conf={summary.MeanConfidence:F2}";
    }

    private static IEnumerable<(CaseOutcome Baseline, CaseOutcome Guarded)> PairByCase(
        IReadOnlyList<CaseOutcome> baseline,
        IReadOnlyList<CaseOutcome> guarded)
    {
        var guardedById = guarded.ToDictionary(x => x.Case.Id, StringComparer.OrdinalIgnoreCase);
        foreach (var b in baseline)
        {
            if (guardedById.TryGetValue(b.Case.Id, out var g))
            {
                yield return (b, g);
            }
        }
    }
}
