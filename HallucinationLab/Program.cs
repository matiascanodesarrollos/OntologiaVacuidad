using HallucinationLab.Backends;
using HallucinationLab.Core;
using HallucinationLab.Guard;

var options = ParseArgs(args);

if (!File.Exists(options.CasesPath))
{
    Console.Error.WriteLine($"Cases file not found: {options.CasesPath}");
    return 1;
}

var backend = CreateBackend(options);
var guard = CreateGuard(options.GuardMode);
var runner = new ExperimentRunner(backend, guard);

var report = await runner.RunAsync(options.CasesPath, CancellationToken.None);
ExperimentRunner.SaveReport(options.OutputPath, report);

PrintSummary(report);
Console.WriteLine($"\nDetailed report saved at: {options.OutputPath}");

return 0;

static void PrintSummary(ExperimentReport report)
{
    Console.WriteLine("Hallucination experiment summary");
    Console.WriteLine($"Backend: {report.BackendName}");
    Console.WriteLine($"Cases: {report.Baseline.CaseCount}");

    Console.WriteLine("\nBaseline");
    Console.WriteLine($"  Avg supported facts : {report.Baseline.AvgSupportedFacts:F2}");
    Console.WriteLine($"  Avg missing facts   : {report.Baseline.AvgMissingFacts:F2}");
    Console.WriteLine($"  Avg forbidden claims: {report.Baseline.AvgForbiddenClaims:F2}");
    Console.WriteLine($"  Avg hallucination   : {report.Baseline.AvgHallucinationRate:P2}");

    Console.WriteLine("\nGuarded");
    Console.WriteLine($"  Avg supported facts : {report.Guarded.AvgSupportedFacts:F2}");
    Console.WriteLine($"  Avg missing facts   : {report.Guarded.AvgMissingFacts:F2}");
    Console.WriteLine($"  Avg forbidden claims: {report.Guarded.AvgForbiddenClaims:F2}");
    Console.WriteLine($"  Avg hallucination   : {report.Guarded.AvgHallucinationRate:P2}");

    var delta = report.Baseline.AvgHallucinationRate - report.Guarded.AvgHallucinationRate;
    Console.WriteLine($"\nHallucination delta (baseline - guarded): {delta:P2}");
}

static Options ParseArgs(string[] args)
{
    var options = new Options
    {
        CasesPath = Path.Combine(AppContext.BaseDirectory, "Samples", "cases.json"),
        ResponsesPath = Path.Combine(AppContext.BaseDirectory, "Samples", "replay-responses.json"),
        OutputPath = Path.Combine(Environment.CurrentDirectory, "hallucination-report.json"),
        Model = "gpt-4o-mini",
        ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? string.Empty,
        BaseUrl = "https://api.openai.com",
        GuardMode = "pass",
        AllowReplayFallback = true
    };

    for (var i = 0; i < args.Length; i++)
    {
        switch (args[i])
        {
            case "--cases" when i + 1 < args.Length:
                options.CasesPath = args[++i];
                break;
            case "--responses" when i + 1 < args.Length:
                options.ResponsesPath = args[++i];
                break;
            case "--out" when i + 1 < args.Length:
                options.OutputPath = args[++i];
                break;
            case "--model" when i + 1 < args.Length:
                options.Model = args[++i];
                break;
            case "--api-key" when i + 1 < args.Length:
                options.ApiKey = args[++i];
                break;
            case "--base-url" when i + 1 < args.Length:
                options.BaseUrl = args[++i];
                break;
            case "--guard" when i + 1 < args.Length:
                options.GuardMode = args[++i];
                break;
            case "--strict-openai":
                options.AllowReplayFallback = false;
                break;
            case "--help":
            case "-h":
                PrintHelp();
                Environment.Exit(0);
                break;
        }
    }

    return options;
}

static void PrintHelp()
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run --project HallucinationLab -- [--cases path] [--out path] [--model gpt-4o-mini] [--guard pass|ontologia]");
    Console.WriteLine("  optional: --api-key <key> (or OPENAI_API_KEY env var), --base-url <url>, --strict-openai");
    Console.WriteLine("  fallback: if no API key and fallback is enabled, ReplayModelBackend uses --responses path");
}

static ITextModelBackend CreateBackend(Options options)
{
    if (!string.IsNullOrWhiteSpace(options.ApiKey))
    {
        return new OpenAiResponsesBackend(options.ApiKey, options.Model, options.BaseUrl);
    }

    if (!options.AllowReplayFallback)
    {
        throw new InvalidOperationException("OPENAI_API_KEY is required when --strict-openai is enabled.");
    }

    if (!File.Exists(options.ResponsesPath))
    {
        throw new FileNotFoundException("Replay responses file not found.", options.ResponsesPath);
    }

    Console.WriteLine("OPENAI_API_KEY not found. Using ReplayModelBackend fallback for no-config execution.");
    return new ReplayModelBackend(options.ResponsesPath);
}

static IOutputGuard CreateGuard(string guardMode)
{
    return guardMode.ToLowerInvariant() switch
    {
        "ontologia" => new OntologiaOutputGuard(),
        _ => new PassThroughGuard()
    };
}

file sealed class Options
{
    public required string CasesPath { get; set; }
    public required string ResponsesPath { get; set; }
    public required string OutputPath { get; set; }
    public required string Model { get; set; }
    public required string ApiKey { get; set; }
    public required string BaseUrl { get; set; }
    public required string GuardMode { get; set; }
    public required bool AllowReplayFallback { get; set; }
}
