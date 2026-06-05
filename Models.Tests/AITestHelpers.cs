using System.Numerics;

namespace Models.Tests;

internal static class AITestHelpers
{
    internal static (bool alucina, string detalleFallo) EvaluarAlucina(
        string verdad,
        string prompt,
        string respuesta,
        double[] referenciaPromptVerdad,
        double[] referenciaRespuestaPrompt,
        double toleranciaDefase,
        double factorUmbralMagnitud,
        double energia,
        bool esperado)
    {
        var alucina = Alucina(
            verdad,
            prompt,
            respuesta,
            referenciaPromptVerdad,
            referenciaRespuestaPrompt,
            toleranciaDefase,
            factorUmbralMagnitud,
            energia,
            out var detalleFallo);

        if (alucina != esperado)
        {
            var truthScore = TruthReferenceEvaluator.Evaluate(respuesta, verdad);
            var (palabra, designacion, aparienciaPregunta) = CrearContextoDiagnostico(
                verdad,
                prompt,
                respuesta,
                referenciaPromptVerdad,
                referenciaRespuestaPrompt,
                energia);

            var carpetaDiagnostico = AITestDiagnostics.GenerarDiagnosticos(
                palabra,
                designacion,
                aparienciaPregunta,
                prompt.Length,
                respuesta.Length,
                toleranciaDefase,
                energia);
            detalleFallo = $"{detalleFallo} | TruthAnchors={truthScore.TruthAnchorsFound}/{truthScore.TruthAnchorsFound + truthScore.MissingTruthAnchors} | Diagnostico={carpetaDiagnostico}";
        }

        return (alucina, detalleFallo);
    }

    internal static bool Alucina(
        string verdad,
        string prompt,
        string respuesta,
        double[] referenciaPromptVerdad,
        double[] referenciaRespuestaPrompt,
        double toleranciaDefase,
        double factorUmbralMagnitud,
        double energia,
        out string detalleFallo)
    {
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
            respuesta,
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

        var alucina = false;
        detalleFallo = $"Sin incumplimiento de umbrales. Fase={toleranciaDefase:F6}, Magnitud={energia:F6}";
        foreach (var omega in designacion.Fourier.Keys)
        {
            for (int tau = 0; tau < prompt.Length; tau++)
            {
                for (int t = 0; t < respuesta.Length; t++)
                {
                    var valorPalabra = palabra.Funcion(tau, t);
                    var valorApariencia = aparienciaPregunta.Funcion(t);
                    var valorDesignacion = designacion.STFT(tau, omega);
                    var deltaMagnitud = Math.Abs(valorApariencia.Magnitude - valorPalabra.Magnitude);
                    var umbralMagnitud = energia * factorUmbralMagnitud;
                    if (deltaMagnitud > umbralMagnitud)
                    {
                        alucina = true;
                        detalleFallo = $"Magnitud: delta={deltaMagnitud:F6} > umbral={umbralMagnitud:F6}, apariencia={valorApariencia.Magnitude:F2}, palabra={valorPalabra.Magnitude:F2}, t={t}, tau={tau}, omega={omega:F6}";
                        break;
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
                        alucina = true;
                        detalleFallo = $"Fase: delta={deltaFase:F6} > umbral={toleranciaDefase:F6}, apariencia={faseApariencia:F2}, designacion={faseDesignacion:F2}, t={t}, omega={omega:F6}";
                        break;
                    }
                }
            }

            if (alucina)
            {
                break;
            }
        }

        return alucina;
    }

    internal static (Palabra palabra, Designacion designacion, Apariencia aparienciaPregunta) CrearContextoDiagnostico(
        string verdad,
        string prompt,
        string respuesta,
        double[] referenciaPromptVerdad,
        double[] referenciaRespuestaPrompt,
        double energia)
    {
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
            respuesta,
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

        return (palabra, designacion, aparienciaPregunta);
    }
}