using System.Text.Json;
using HallucinationLab.Backends;
using HallucinationLab.Eval;
using HallucinationLab.Guard;

namespace HallucinationLab.Core;

public sealed class ExperimentRunner
{
    private readonly ITextModelBackend backend;
    private readonly IOutputGuard guard;

    public ExperimentRunner(ITextModelBackend backend, IOutputGuard guard)
    {
        this.backend = backend;
        this.guard = guard;
    }

    public async Task<ExperimentReport> RunAsync(string casesPath, CancellationToken cancellationToken)
    {
        var cases = LoadCases(casesPath);
        var results = new List<CaseResult>(cases.Count);

        foreach (var promptCase in cases)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var baseline = await backend.GenerateAsync(promptCase.Prompt, cancellationToken);
            var guarded = guard.Apply(promptCase.Truth, promptCase.Prompt, baseline);

            var baselineScore = HallucinationEvaluator.Evaluate(baseline, promptCase.Truth);
            var guardedScore = HallucinationEvaluator.Evaluate(guarded, promptCase.Truth);

            results.Add(new CaseResult
            {
                Id = promptCase.Id,
                Prompt = promptCase.Prompt,
                BaselineOutput = baseline,
                GuardedOutput = guarded,
                BaselineScore = baselineScore,
                GuardedScore = guardedScore
            });
        }

        return new ExperimentReport
        {
            BackendName = backend.Name,
            Cases = results,
            Baseline = HallucinationEvaluator.Aggregate(results.Select(r => r.BaselineScore)),
            Guarded = HallucinationEvaluator.Aggregate(results.Select(r => r.GuardedScore))
        };
    }

    public static void SaveReport(string outputPath, ExperimentReport report)
    {
        var json = JsonSerializer.Serialize(report, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(outputPath, json);
    }

    private static List<PromptCase> LoadCases(string casesPath)
    {
        var json = File.ReadAllText(casesPath);
        return JsonSerializer.Deserialize<List<PromptCase>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })
            ?? new List<PromptCase>();
    }
}
