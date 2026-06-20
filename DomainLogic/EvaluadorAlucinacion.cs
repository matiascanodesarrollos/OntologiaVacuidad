namespace DomainLogic;

public class EvaluadorAlucinacion
{
    private readonly AIDiagnostics _diagnostics = new AIDiagnostics();

    public (bool alucina, string detalleFallo) Evaluar(
        string verdad,
        string prompt,
        string respuesta,
        double[] referenciaPromptVerdad,
        double[] referenciaRespuestaPrompt,
        double energia,
        double defasePermitido, 
        double frecuenciaAngularRespiracion,
        double tiempoRespuesta,
        bool esperado)
    {
        var contextoBuilder = new ContextBuilder(verdad)
                .ConPrompt(prompt, referenciaPromptVerdad, frecuenciaAngularRespiracion)
                .ConRespuesta(respuesta, referenciaRespuestaPrompt, frecuenciaAngularRespiracion, tiempoRespuesta);

        var alucina = Alucina(
            contextoBuilder,       
            energia,
            defasePermitido,
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
        double energia,
        double defasePermitido,
        out string detalleFallo)
    {
        detalleFallo = $"Sin incumplimiento de umbrales.";       
            
        var tMax = Math.Max(contextoBuilder.Prompt.Length, contextoBuilder.Respuesta.Length);
        for(var t = 0.0; t <= tMax; t +=0.1)
        {
            
        }
        
        return false;
    }
}