namespace Models.Tests;

public class AITestHelpers
{
    private readonly AITestDiagnostics _diagnostics = new AITestDiagnostics();

    public (bool alucina, string detalleFallo) EvaluarAlucina(
        string verdad,
        string prompt,
        string respuesta,
        double[] referenciaPromptVerdad,
        double[] referenciaRespuestaPrompt,
        double toleranciaRacional,
        int maxDenominador,
        double factorUmbralMagnitud,
        double energia,
        double oscilacionMaximaPermitida,
        bool esperado)
    {
        var contextoBuilder = new ContextBuilder(verdad)
                .ConPrompt(prompt, referenciaPromptVerdad, velocidadGrupo: 1.0)
                .ConRespuesta(respuesta, referenciaRespuestaPrompt, velocidadGrupo: 1.0);

        var alucina = Alucina(
            contextoBuilder,
            toleranciaRacional,
            maxDenominador,
            factorUmbralMagnitud,            
            energia,
            oscilacionMaximaPermitida,
            out var detalleFallo);

        if (alucina != esperado)
        {
            var carpetaDiagnostico = _diagnostics.GenerarDiagnosticos(
                contextoBuilder);
            detalleFallo = $"{detalleFallo} | Diagnostico={carpetaDiagnostico}";
        }

        return (alucina, detalleFallo);
    }

    private bool Alucina(
        ContextBuilder contextoBuilder,
        double toleranciaRacional,
        int maxDenominador,
        double factorUmbralMagnitud,
        double energia,
        double oscilacionMaximaPermitida,
        out string detalleFallo)
    {
        var magnitudMaxima = energia * factorUmbralMagnitud;
        var magnitudMinima = energia / factorUmbralMagnitud;
        detalleFallo = $"Sin incumplimiento de umbrales. Magnitud Maxima={magnitudMaxima:F6}, Magnitud Minima={magnitudMinima:F6}";

        var amplitudRespuesta = contextoBuilder.AparienciaRespuesta.Amplitud.Value.Magnitude;
        if (amplitudRespuesta > magnitudMaxima || amplitudRespuesta < magnitudMinima)
        {
            detalleFallo = $"Incumplimiento en magnitud. AmplitudRespuesta={amplitudRespuesta:F6} fuera del rango [{magnitudMinima:F6}, {magnitudMaxima:F6}]";
            return true;
        }

        var razonFrecuencial = contextoBuilder.AparienciaPromt.FrecuenciaAngular
            / contextoBuilder.AparienciaRespuesta.FrecuenciaAngular;
        var armoniza = EsRazonRacional(
            razonFrecuencial,
            toleranciaRacional,
            maxDenominador);
        if (!armoniza)
        {
            detalleFallo =
                $"Armonizacion frecuencial insuficiente. RazonOmega={razonFrecuencial:F6}, tolerancia={toleranciaRacional:F6}, maximoDenominador={maxDenominador}.";
            return true;
        }

        var designacion = contextoBuilder.CrearDesignacion();
            
        var tMax = Math.Max(contextoBuilder.Prompt.Length, contextoBuilder.Respuesta.Length);
        foreach (var omega in contextoBuilder.NombrePromt.Fourier.Keys)
        {
            double? valorInicial = null;
            for (var tau = 0.0; tau <= tMax; tau += 0.01)
            {
                var valorSTFT = designacion.STFT(tau, omega);
                if (valorInicial is null && valorSTFT.Magnitude > oscilacionMaximaPermitida)
                {
                    valorInicial = valorSTFT.Magnitude;
                }

                if (valorInicial is not null
                    && Math.Abs(valorInicial.Value - valorSTFT.Magnitude) > oscilacionMaximaPermitida)
                {
                    detalleFallo = $"Incumplimiento en oscilacion máxima. Omega={omega:F6} no se cancela, tau={tau:F2} valorSTFT={valorSTFT.Magnitude:F6}, valorInicial={valorInicial.Value:F6}.";
                    return true;
                }
            }
        }

        return false;
    }

    private bool EsRazonRacional(
        double razon,
        double tolerancia,
        int maxDenominador)
    {
        var mejorError = double.PositiveInfinity;
        if (!double.IsFinite(razon))
        {
            return false;
        }

        for (var denominador = 1; denominador <= maxDenominador; denominador++)
        {
            var numerador = (int)Math.Round(razon * denominador);
            var aproximacion = (double)numerador / denominador;
            var error = Math.Abs(razon - aproximacion);

            if (error < mejorError)
            {
                mejorError = error;
            }
        }

        return mejorError <= tolerancia;
    }
}