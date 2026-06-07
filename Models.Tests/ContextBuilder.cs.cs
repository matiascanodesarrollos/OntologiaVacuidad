using System.Numerics;

namespace Models.Tests;

public class ContextBuilder
{
    public readonly double ToleranciaDefase;
    public readonly double FactorUmbralMagnitud;
    private readonly string _verdad;
    public string Prompt { get; private set; }
    public readonly double EnergiaPrompt;
    private Nombre _nombrePromt;
    public Apariencia AparienciaPregunta { get; private set; }
    public string Respuesta { get; private set; }
    private Nombre _nombreRespuesta;    
    public Apariencia AparienciaRespuesta { get; private set; }
    public Designacion Designacion { get; private set; }    

    public ContextBuilder(double energia, string verdad, double toleranciaDefase, double factorUmbralMagnitud)
    {
        EnergiaPrompt = energia;
        _verdad = verdad;
        ToleranciaDefase = toleranciaDefase;
        FactorUmbralMagnitud = factorUmbralMagnitud;
    }

    public ContextBuilder ConPrompt(string prompt, double[] referenciaPromptVerdad, double velocidadGrupo)
    {
        Prompt = prompt;

        var ventana = GetVentana(referenciaPromptVerdad);
        _nombrePromt = new Nombre(
            prompt,
            _verdad,
            ventana,
            velocidadGrupo);
        AparienciaPregunta = new Apariencia(_nombrePromt);

        return this;
    }    

    public ContextBuilder ConRespuesta(string respuesta, double[] referenciaRespuestaPrompt, double velocidadGrupo)
    {
        if (_nombrePromt == null || Prompt == null)
        {
            throw new InvalidOperationException("Debe configurar el prompt antes de la respuesta.");
        }

        Respuesta = respuesta;
        var ventana = GetVentana(referenciaRespuestaPrompt);
        _nombreRespuesta = new Nombre(
            respuesta,
            _verdad,
            ventana,
            velocidadGrupo);
        AparienciaRespuesta = new Apariencia(_nombreRespuesta);

        return this;
    }

    public Designacion ConstruirDesignacion()
    {
        if (_nombrePromt == null || _nombreRespuesta == null)
        {
            throw new InvalidOperationException("Debe configurar el prompt y la respuesta antes de construir la designación.");
        }

        Designacion = new Designacion(AparienciaPregunta, _nombreRespuesta);
        return Designacion;
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
