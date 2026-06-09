using System.Numerics;

namespace Models.Tests;

public class ContextBuilder
{
    private readonly string _verdad;
    public string Prompt { get; private set; }
    public Nombre NombrePromt;
    public Apariencia AparienciaPromt;
    public string Respuesta { get; private set; }
    public Nombre NombreRespuesta;
    public Apariencia AparienciaRespuesta;

    public ContextBuilder(string verdad)
    {
        _verdad = verdad;
    }

    public ContextBuilder ConPrompt(string prompt, double[] referenciaPromptVerdad, double velocidadGrupo)
    {
        Prompt = prompt;

        var ventana = GetVentana(referenciaPromptVerdad);
        NombrePromt = new Nombre(
            prompt,
            _verdad,
            ventana,
            velocidadGrupo);
        AparienciaPromt = new Apariencia(NombrePromt);

        return this;
    }    

    public ContextBuilder ConRespuesta(string respuesta, double[] referenciaRespuestaPrompt, double velocidadGrupo)
    {
        if (NombrePromt == null || Prompt == null)
        {
            throw new InvalidOperationException("Debe configurar el prompt antes de la respuesta.");
        }

        Respuesta = respuesta;
        var ventana = GetVentana(referenciaRespuestaPrompt);
        NombreRespuesta = new Nombre(
            respuesta,
            $"{_verdad}:{Prompt}",
            ventana,
            velocidadGrupo);
        AparienciaRespuesta = new Apariencia(NombreRespuesta);

        return this;
    }

    public Designacion CrearDesignacion()
    {
        if (NombrePromt == null || NombreRespuesta == null)
        {
            throw new InvalidOperationException("Debe configurar el prompt y la respuesta antes de crear la designación.");
        }

        return new Designacion(AparienciaPromt, NombreRespuesta);
    }

    private Func<double, Complex> GetVentana(double[] referencia) => t =>
    {
        var indice = (int)t;
        if (indice >= 0 && indice < referencia.Length)
        {
            return referencia[indice];
        }

        return 0;
    };
}
