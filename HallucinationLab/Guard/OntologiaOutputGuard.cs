using System.Numerics;
using System.Text.RegularExpressions;

namespace HallucinationLab.Guard;

public sealed class OntologiaOutputGuard : IOutputGuard
{
    private const string AbstentionResponse = "Me abstengo: no hay acoplamiento suficiente.";
    private const double MinimumExpectedCoupling = 0.45;
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "a",
        "an",
        "and",
        "de",
        "del",
        "el",
        "in",
        "is",
        "la",
        "las",
        "los",
        "of",
        "on",
        "or",
        "the",
        "to",
        "un",
        "una",
        "y"
    };

    public string Name => "OntologiaOutputGuard";

    public string Apply(
        string prompt, 
        string rawOutput, 
        IReadOnlyList<string> expectedFacts, 
        IReadOnlyList<string> forbiddenClaims)
    {
        if (string.IsNullOrWhiteSpace(rawOutput))
        {
            return AbstentionResponse;
        }

        var promptAppearance = CreatePromptAppearance(prompt);
        var outputName = CreateNombreFromText(ExtractOutputName(rawOutput), promptAppearance.Esencia.VelocidadGrupo);
        var outputDesignation = Designacion.Designar(promptAppearance, outputName);

        var expectedNames = expectedFacts
            .Where(fact => !string.IsNullOrWhiteSpace(fact))
            .Select(fact => CreateNombreFromText(fact, promptAppearance.Esencia.VelocidadGrupo))
            .ToList();
        var forbiddenNames = forbiddenClaims
            .Where(claim => !string.IsNullOrWhiteSpace(claim))
            .Select(claim => CreateNombreFromText(claim, promptAppearance.Esencia.VelocidadGrupo))
            .ToList();

        var bestExpectedCoupling = expectedNames.Count == 0
            ? 1.0
            : expectedNames.Max(name => MeasureCoupling(promptAppearance, outputDesignation, name));
        var bestForbiddenCoupling = forbiddenNames.Count == 0
            ? 0.0
            : forbiddenNames.Max(name => MeasureCoupling(promptAppearance, outputDesignation, name));

        var shouldAbstain = bestForbiddenCoupling >= bestExpectedCoupling
            || (expectedNames.Count > 0 && bestExpectedCoupling < MinimumExpectedCoupling);

        return shouldAbstain ? AbstentionResponse : rawOutput;
    }

    private static Apariencia CreatePromptAppearance(string prompt)
    {
        var tokens = Tokenize(prompt);
        var words = tokens.Count == 0
            ? new[] { CreatePalabra("silencio", 1.0) }
            : tokens.Select(token => CreatePalabra(token)).ToArray();

        return Apariencia.Aparecer(words, ComputeVelocityGroup(tokens));
    }

    private static Nombre CreateNombreFromText(string text, double velocityGroup)
    {
        var normalized = NormalizeText(text);
        return new Nombre(normalized, velocityGroup, CreateWindow(normalized));
    }

    private static double MeasureCoupling(Apariencia promptAppearance, Designacion outputDesignation, Nombre candidateName)
    {
        var candidateDesignation = Designacion.Designar(promptAppearance, candidateName);
        var lexicalCoupling = ComputeTokenOverlap(outputDesignation.Texto, candidateName.Texto);
        var spectralGap = Math.Abs(outputDesignation.FrecuenciaAngular - candidateDesignation.FrecuenciaAngular);
        var spectralCoupling = 1.0 / (1.0 + spectralGap);

        return (0.65 * lexicalCoupling) + (0.35 * spectralCoupling);
    }

    private static string ExtractOutputName(string rawOutput)
    {
        var candidate = Regex
            .Split(rawOutput, @"[\r\n.!?;:]+")
            .Select(NormalizeText)
            .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

        return string.IsNullOrWhiteSpace(candidate)
            ? NormalizeText(rawOutput)
            : candidate;
    }

    private static Palabra CreatePalabra(string token)
    {
        var normalized = NormalizeText(token);
        return CreatePalabra(normalized, ComputeVelocityGroup(new[] { normalized }));
    }

    private static Palabra CreatePalabra(string token, double velocityGroup)
    {
        return new Palabra(
            token,
            ComputeAngularFrequency(token),
            CreateWindow(token),
            velocityGroup);
    }

    private static Func<double, Complex> CreateWindow(string text)
    {
        var width = Math.Max(1.0, NormalizeText(text).Length / 3.0);
        return t => new Complex(Math.Exp(-((t * t) / (2.0 * width * width))), 0.0);
    }

    private static double ComputeVelocityGroup(IEnumerable<string> tokens)
    {
        var tokenList = tokens.Where(token => !string.IsNullOrWhiteSpace(token)).ToList();
        if (tokenList.Count == 0)
        {
            return 1.0;
        }

        return tokenList.Average(token => (double)token.Length);
    }

    private static double ComputeAngularFrequency(string token)
    {
        var normalized = NormalizeText(token);
        if (normalized.Length == 0)
        {
            return 0.0;
        }

        unchecked
        {
            var hash = 17;
            foreach (var character in normalized)
            {
                hash = (hash * 31) + character;
            }

            var bucket = Math.Abs(hash % 321);
            return -8.0 + (16.0 * bucket / 320.0);
        }
    }

    private static double ComputeTokenOverlap(string left, string right)
    {
        var leftTokens = Tokenize(left)
            .Where(token => !StopWords.Contains(token))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var rightTokens = Tokenize(right)
            .Where(token => !StopWords.Contains(token))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (leftTokens.Count == 0 || rightTokens.Count == 0)
        {
            return 0.0;
        }

        var intersection = leftTokens.Intersect(rightTokens, StringComparer.OrdinalIgnoreCase).Count();
        return intersection / (double)rightTokens.Count;
    }

    private static List<string> Tokenize(string text)
    {
        return Regex.Matches(NormalizeText(text), @"[\p{L}\p{Nd}]+")
            .Select(match => match.Value)
            .ToList();
    }

    private static string NormalizeText(string text)
    {
        return Regex.Replace(text ?? string.Empty, @"\s+", " ").Trim().ToLowerInvariant();
    }
}
