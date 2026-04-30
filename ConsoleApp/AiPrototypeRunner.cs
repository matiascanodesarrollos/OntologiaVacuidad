using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ConsoleApp;

internal static class AiPrototypeRunner
{
    public static async Task<int> RunAsync(string[] args, ILogger logger)
    {
        if (args.Length == 0 || args[0].Equals("help", StringComparison.OrdinalIgnoreCase))
        {
            PrintHelp(logger);
            return 0;
        }

        var command = args[0].ToLowerInvariant();
        var remaining = args.Skip(1).ToArray();
        var pythonPath = ResolvePythonPath();
        var rootDir = FindRepositoryRoot();
        var prototypeRoot = Path.Combine(rootDir, "AIPrototype");
        var waveTruthDir = Path.Combine(prototypeRoot, "wavetruth");

        if (!File.Exists(pythonPath) && !pythonPath.Equals("python.exe", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogError("No se encontro Python. Configure ONTOLOGIA_PYTHON o instale Python 3.12 en una ruta conocida.");
            return 1;
        }

        return command switch
        {
            "install" => await RunProcessAsync(pythonPath, $"-m pip install -r \"{Path.Combine(prototypeRoot, "requirements.txt")}\"", prototypeRoot, logger),
            "train" => await RunProcessAsync(pythonPath, BuildTrainArguments(Path.Combine(waveTruthDir, "wavetruth.py"), remaining), prototypeRoot, logger),
            "eval" => await RunProcessAsync(pythonPath, $"\"{Path.Combine(waveTruthDir, "experiment.py")}\"", prototypeRoot, logger),
            _ => UnknownCommand(command, logger),
        };
    }

    private static int UnknownCommand(string command, ILogger logger)
    {
        logger.LogError("Comando AI no reconocido: {Command}", command);
        PrintHelp(logger);
        return 1;
    }

    private static string BuildTrainArguments(string scriptPath, string[] remaining)
    {
        var baseArgs = new List<string> { $"\"{scriptPath}\"" };
        if (remaining.Length == 0)
        {
            baseArgs.Add("--trainer.max_iters=20");
            baseArgs.Add("--trainer.batch_size=8");
            baseArgs.Add("--trainer.num_workers=0");
        }
        else
        {
            baseArgs.AddRange(remaining);
        }

        return string.Join(" ", baseArgs);
    }

    private static string ResolvePythonPath()
    {
        var envPath = Environment.GetEnvironmentVariable("ONTOLOGIA_PYTHON");
        if (!string.IsNullOrWhiteSpace(envPath))
        {
            return envPath;
        }

        var candidates = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "Python", "Python312", "python.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "Python", "Python311", "python.exe"),
            "python.exe"
        };

        return candidates.FirstOrDefault(File.Exists) ?? candidates.Last();
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "OntologiaVacuidad.sln")))
            {
                return current.FullName;
            }

            if (Directory.Exists(Path.Combine(current.FullName, "AIPrototype")) && Directory.Exists(Path.Combine(current.FullName, "ConsoleApp")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("No se pudo ubicar la raiz del proyecto OntologiaVacuidad.");
    }

    private static async Task<int> RunProcessAsync(string fileName, string arguments, string workingDirectory, ILogger logger)
    {
        logger.LogInformation("Ejecutando: {FileName} {Arguments}", fileName, arguments);

        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = new Process { StartInfo = startInfo };
        process.OutputDataReceived += (_, eventArgs) =>
        {
            if (!string.IsNullOrEmpty(eventArgs.Data))
            {
                logger.LogInformation("{Line}", eventArgs.Data);
            }
        };
        process.ErrorDataReceived += (_, eventArgs) =>
        {
            if (!string.IsNullOrEmpty(eventArgs.Data))
            {
                logger.LogError("{Line}", eventArgs.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();
        logger.LogInformation("Proceso finalizado con codigo {ExitCode}", process.ExitCode);
        return process.ExitCode;
    }

    private static void PrintHelp(ILogger logger)
    {
        logger.LogInformation("Comandos AI disponibles:");
        logger.LogInformation("  ai install");
        logger.LogInformation("  ai train [overrides]");
        logger.LogInformation("  ai eval");
        logger.LogInformation("Variable opcional: ONTOLOGIA_PYTHON para apuntar a un python.exe especifico.");
    }
}