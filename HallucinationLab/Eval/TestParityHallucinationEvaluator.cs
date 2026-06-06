using System.Numerics;
using HallucinationLab.Core;

namespace HallucinationLab.Eval;

public static class TestParityHallucinationEvaluator
{
    public static bool DetectHallucination(PromptCase promptCase, string output)
    {
        if (output.StartsWith("Me abstengo:", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var referenciaPromptVerdad = promptCase.ReferenciaPromptVerdad.ToArray();
        var referenciaRespuestaPrompt = promptCase.ReferenciaRespuestaPrompt.ToArray();

        if (output.Length != referenciaRespuestaPrompt.Length)
        {
            return true;
        }

        var prompt = promptCase.Prompt;
        var verdad = promptCase.Truth;
        var energia = prompt.Length;
        var toleranciaDefase = promptCase.ToleranciaDefase;
        var factorUmbralMagnitud = promptCase.FactorUmbralMagnitud;

        Func<double, Complex> funcionTemporalPregunta = t =>
        {
            var indice = (int)t;
            if (indice >= 0 && indice < referenciaPromptVerdad.Length)
            {
                return referenciaPromptVerdad[indice];
            }

            return 0;
        };

        var nombrePregunta = new Nombre(
            prompt,
            verdad,
            funcionTemporalPregunta,
            1.0);

        Func<double, Complex> funcionTemporalRespuesta = t =>
        {
            var indice = (int)t;
            if (indice >= 0 && indice < referenciaRespuestaPrompt.Length)
            {
                return referenciaRespuestaPrompt[indice];
            }

            return 0;
        };

        var nombreRespuesta = new Nombre(
            output,
            prompt,
            funcionTemporalRespuesta,
            1.0);

        var aparienciaPregunta = new Apariencia(
            nombrePregunta.Fourier.Sum(p => p.Key),
            t =>
            {
                if (t == 0)
                {
                    return new Complex(energia, 0.0);
                }

                if (t > prompt.Length || t < 0)
                {
                    return Complex.Zero;
                }

                return new Complex(energia * Math.Exp(-t), energia * t);
            },
            energia);

        var palabra = nombrePregunta.Mostrarse(aparienciaPregunta);

        var designacion = new Designacion(
            aparienciaPregunta,
            nombreRespuesta);

        foreach (var omega in designacion.Fourier.Keys)
        {
            for (int tau = 0; tau < prompt.Length; tau++)
            {
                for (int t = 0; t < output.Length; t++)
                {
                    var valorPalabra = palabra.Funcion(tau, t);
                    var valorApariencia = aparienciaPregunta.Funcion(t);
                    var valorDesignacion = designacion.STFT(tau, omega);

                    var deltaMagnitud = Math.Abs(valorApariencia.Magnitude - valorPalabra.Magnitude);
                    var umbralMagnitud = energia * factorUmbralMagnitud;
                    if (deltaMagnitud > umbralMagnitud)
                    {
                        return true;
                    }

                    var umbralMagnitudFase = Math.Max(1e-6, energia * 0.01);
                    if (!double.IsFinite(valorApariencia.Magnitude)
                        || !double.IsFinite(valorPalabra.Magnitude)
                        || valorApariencia.Magnitude <= umbralMagnitudFase
                        || valorPalabra.Magnitude <= umbralMagnitudFase)
                    {
                        continue;
                    }

                    var faseApariencia = Math.Abs(valorApariencia.Phase);
                    var faseDesignacion = Math.Abs(valorDesignacion.Phase);
                    var deltaFase = faseApariencia - faseDesignacion;
                    if (deltaFase > toleranciaDefase)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}