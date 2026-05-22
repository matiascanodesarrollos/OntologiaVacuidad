using System.Numerics;
using System.Text.RegularExpressions;

namespace HallucinationLab.Guard;

public sealed class OntologiaOutputGuard : IOutputGuard
{
    private static readonly Regex TokenRegex = new("[A-Za-z0-9_]+", RegexOptions.Compiled);

    public string Name => "OntologiaOutputGuard";

    public string Apply(string prompt, string rawOutput)
    {
        var promptWords = Tokenize(prompt).ToList();
        var outputWords = Tokenize(rawOutput).ToList();

        if (outputWords.Count == 0)
        {
            return rawOutput;
        }

        var promptFrequencies = promptWords.Select(ToAngularFrequency).ToArray();
        var promptCenter = promptFrequencies.Length == 0 ? 1.0 : promptFrequencies.Average();
        var promptSpread = promptFrequencies.Length <= 1
            ? 1.0
            : Math.Sqrt(promptFrequencies.Select(v => Math.Pow(v - promptCenter, 2.0)).Average());
        var threshold = Math.Max(0.9, 1.6 * promptSpread);

        var accepted = new List<string>(outputWords.Count);
        foreach (var word in outputWords)
        {
            var frequency = ToAngularFrequency(word);
            var distance = Math.Abs(frequency - promptCenter);
            if (distance <= threshold)
            {
                accepted.Add(word);
            }
            else
            {
                accepted.Add("[verificar]");
            }
        }

        // Build an ontologia signal as a side-effect safety check: if unstable, return a conservative output.
        if (!IsNumericallyStableSignal(accepted))
        {
            return "[salida no confiable: revise fuentes]";
        }

        return string.Join(" ", accepted);
    }

    private static bool IsNumericallyStableSignal(IReadOnlyList<string> words)
    {
        if (words.Count == 0)
        {
            return true;
        }

        try
        {
            var palabras = words
                .Where(static w => !string.Equals(w, "[verificar]", StringComparison.OrdinalIgnoreCase))
                .Select(w => new Palabra(w, ToAngularFrequency(w), GaussianWindow))
                .ToArray();

            if (palabras.Length == 0)
            {
                return true;
            }

            var apariencia = Apariencia.Aparecer(palabras);
            var nombre = new Nombre("guarded", 0.0, GaussianWindow);
            var designacion = Designacion.Designar(apariencia, nombre);

            var amplitudeIsFinite = double.IsFinite(apariencia.Amplitud);
            var velocityIsFinite = double.IsFinite(designacion.FrecuenciaAngular);
            var stft = designacion.STFT((0.5, 1.0));
            var stftIsFinite = double.IsFinite(stft.Real) && double.IsFinite(stft.Imaginary);

            return amplitudeIsFinite && velocityIsFinite && stftIsFinite;
        }
        catch
        {
            return false;
        }
    }

    private static IEnumerable<string> Tokenize(string value)
    {
        foreach (Match match in TokenRegex.Matches(value))
        {
            yield return match.Value;
        }
    }

    private static double ToAngularFrequency(string token)
    {
        unchecked
        {
            var hash = token.ToLowerInvariant().Aggregate(17, (acc, ch) => (acc * 31) + ch);
            var normalized = Math.Abs(hash % 5800) / 1000.0;
            return 0.2 + normalized;
        }
    }

    private static Complex GaussianWindow(double t)
    {
        return new Complex(Math.Exp(-(t * t) / 2.0), 0.0);
    }
}
