using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EpistemicGuard;

public static class TextProcessing
{
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "a", "al", "algo", "con", "de", "del", "el", "en", "es", "esta", "este", "la", "las", "lo", "los",
        "no", "para", "por", "que", "se", "su", "sus", "un", "una", "y"
    };

    private static readonly HashSet<string> NegationTokens = new(StringComparer.OrdinalIgnoreCase)
    {
        "no", "nunca", "jamas", "sin"
    };

    public static HashSet<string> Tokenize(string text)
    {
        var cleaned = new StringBuilder(text.Length);
        foreach (var ch in text.ToLowerInvariant())
        {
            cleaned.Append(char.IsLetterOrDigit(ch) ? ch : ' ');
        }

        return cleaned
            .ToString()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(token => !StopWords.Contains(token))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public static bool HasNegation(string text)
    {
        var tokens = Tokenize(text);
        return tokens.Any(NegationTokens.Contains);
    }

    public static double JaccardSimilarity(HashSet<string> left, HashSet<string> right)
    {
        if (left.Count == 0 && right.Count == 0)
        {
            return 0.0;
        }

        var intersection = left.Intersect(right, StringComparer.OrdinalIgnoreCase).Count();
        var union = left.Union(right, StringComparer.OrdinalIgnoreCase).Count();
        return union == 0 ? 0.0 : (double)intersection / union;
    }
}
