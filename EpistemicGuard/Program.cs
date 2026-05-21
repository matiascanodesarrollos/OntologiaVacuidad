using EpistemicGuard;
using System.IO;

var runDemo = args.Length == 0 || args.Contains("--demo", StringComparer.OrdinalIgnoreCase);
var runBenchmark = args.Length == 0 || args.Contains("--benchmark", StringComparer.OrdinalIgnoreCase);

if (runDemo)
{
	RunDemo();
}

if (runBenchmark)
{
	RunBenchmark();
}

static void RunDemo()
{
	var projector = new EvidenceProjector();
	var policy = new PolicyEngine();
	var metrics = new MetricsTracker();

	Console.WriteLine("EpistemicGuard demo");
	Console.WriteLine("==================");

	foreach (var claim in DemoData.Claims)
	{
		var projection = projector.Project(claim, DemoData.Corpus, topK: 3);
		var decision = policy.Decide(projection);
		metrics.Add(projection, decision);

		Console.WriteLine();
		Console.WriteLine($"Claim: {claim.Text}");
		Console.WriteLine($"Verdict: {decision.Verdict}");
		Console.WriteLine($"Confidence: {decision.Confidence:F2}");
		Console.WriteLine($"Support: {projection.SupportScore:F3} | Contradiction: {projection.ContradictionScore:F3} | Coverage: {projection.CoverageScore:F3}");
		Console.WriteLine($"Rationale: {decision.Rationale}");
		Console.WriteLine($"Evidence IDs: {(decision.EvidenceIds.Count == 0 ? "(none)" : string.Join(", ", decision.EvidenceIds))}");
	}

	var snapshot = metrics.Snapshot();

	Console.WriteLine();
	Console.WriteLine("Metrics");
	Console.WriteLine("-------");
	Console.WriteLine($"UnsupportedAssertionRate: {snapshot.UnsupportedAssertionRate:P2}");
	Console.WriteLine($"ConflictRate: {snapshot.ConflictRate:P2}");
	Console.WriteLine($"MeanConfidence: {snapshot.MeanConfidence:F2}");
	Console.WriteLine();
}

static void RunBenchmark()
{
	var runner = new EvaluationRunner();

	var baseline = runner.Run(
		"Baseline",
		new BaselineDecisionEngine(),
		BenchmarkData.Suite,
		BenchmarkData.Corpus);

	var guarded = runner.Run(
		"Guarded",
		new GuardedDecisionEngineAdapter(),
		BenchmarkData.Suite,
		BenchmarkData.Corpus);

	var report = Reporter.BuildComparativeReport(
		baseline.Summary,
		guarded.Summary,
		baseline.Outcomes,
		guarded.Outcomes);

	Console.WriteLine("Benchmark");
	Console.WriteLine("=========");
	Console.WriteLine(report);

	var outputPath = Path.Combine(ResolveProjectDirectory(), "benchmark-latest.md");
	File.WriteAllText(outputPath, report);
	Console.WriteLine($"Reporte guardado en: {outputPath}");
}

static string ResolveProjectDirectory()
{
	var current = new DirectoryInfo(AppContext.BaseDirectory);

	while (current is not null)
	{
		if (File.Exists(Path.Combine(current.FullName, "EpistemicGuard.csproj")))
		{
			return current.FullName;
		}

		current = current.Parent;
	}

	return Directory.GetCurrentDirectory();
}
