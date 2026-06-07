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
        double umbralPlv,
        double factorUmbralMagnitud,
        double energia,
        bool esperado)
    {
        var contextoBuilder = new ContextBuilder(verdad)
                .ConPrompt(prompt, referenciaPromptVerdad, velocidadGrupo: 1.0)
                .ConRespuesta(respuesta, referenciaRespuestaPrompt, velocidadGrupo: 1.0);

        var alucina = Alucina(
            contextoBuilder,
            umbralPlv,
            factorUmbralMagnitud,
            energia,
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
        double umbralPlv,
        double factorUmbralMagnitud,
        double energia,
        out string detalleFallo)
    {
        var magnitudMaxima = energia;
        var magnitudMinima = energia / factorUmbralMagnitud;
        detalleFallo = $"Sin incumplimiento de umbrales. Magnitud Maxima={magnitudMaxima:F6}, Magnitud Minima={magnitudMinima:F6}";

        var amplitudRespuesta = contextoBuilder.AparienciaRespuesta.Amplitud.Value.Magnitude;
        if (amplitudRespuesta > magnitudMaxima || amplitudRespuesta < magnitudMinima)
        {
            detalleFallo = $"Incumplimiento en magnitud. AmplitudRespuesta={amplitudRespuesta:F6} fuera del rango [{magnitudMinima:F6}, {magnitudMaxima:F6}]";
            return true;
        }
        
        if (!EstaArmonizada(
            contextoBuilder,
            umbralPlv,
            out var detalleArmonia))
        {
            detalleFallo = detalleArmonia;
            return true;
        }

        return false;
    }

    private bool EstaArmonizada(
        ContextBuilder contextoBuilder,
        double umbralPlv,
        out string detalleArmonia)
    {
        var plv = CalcularPlv(contextoBuilder);
        var armoniza = plv >= umbralPlv;

        detalleArmonia =
            $"Armonizacion frecuencial insuficiente. " +
            $"PLV={plv:F6}, umbralPLV={umbralPlv:F2}.";

        return armoniza;
    }

    private double CalcularPlv(ContextBuilder contextoBuilder)
    {
        var tMax = Math.Max(contextoBuilder.Prompt.Length, contextoBuilder.Respuesta.Length);
        if (tMax <= 0)
        {
            return 0.0;
        }

        var sumaCos = 0.0;
        var sumaSin = 0.0;
        for (var t = 0; t < tMax; t++)
        {
            var fasePromt = contextoBuilder.AparienciaPromt.Funcion(t).Phase;
            var faseRespuesta = contextoBuilder.AparienciaRespuesta.Funcion(t).Phase;
            var deltaFase = fasePromt - faseRespuesta;

            sumaCos += Math.Cos(deltaFase);
            sumaSin += Math.Sin(deltaFase);
        }

        return Math.Sqrt((sumaCos * sumaCos) + (sumaSin * sumaSin)) / tMax;
    }
}