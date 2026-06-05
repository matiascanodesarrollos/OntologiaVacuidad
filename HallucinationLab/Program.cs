using HallucinationLab.Backends;
using HallucinationLab.Core;
using HallucinationLab.Guard;

var options = ParseArgs(args);

if (!File.Exists(options.CasesPath))
{
    Console.Error.WriteLine($"Cases file not found: {options.CasesPath}");
    return 1;
}

var backend = new ReplayModelBackend(options.ResponsesPath);
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
    Console.WriteLine($"  Accuracy vs expected: {report.Baseline.ExpectationAccuracy:P2}");
    Console.WriteLine($"  Avg hallucination   : {report.Baseline.AvgHallucinationRate:P2}");

    Console.WriteLine("\nGuarded");
    Console.WriteLine($"  Accuracy vs expected: {report.Guarded.ExpectationAccuracy:P2}");
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
        GuardMode = "ontologia",
    };

    for (var i = 0; i < args.Length; i++)
    {
        switch (args[i])
        {
            case "--guard" when i + 1 < args.Length:
                options.GuardMode = args[++i];
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
    Console.WriteLine("  dotnet run --project HallucinationLab -- [--guard pass|ontologia]");
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
    public required string GuardMode { get; set; }
}
