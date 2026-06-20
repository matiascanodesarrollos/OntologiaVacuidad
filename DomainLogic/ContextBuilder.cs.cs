using System.Numerics;

namespace DomainLogic;

public class ContextBuilder
{
    private readonly string _verdad;
    public string Prompt { get; private set; }
    public Nombre NombrePromt;
    public string Respuesta { get; private set; }
    public Nombre NombreRespuesta;

    public ContextBuilder(string verdad)
    {
        _verdad = verdad;
    }

    public ContextBuilder ConPrompt(
        string prompt, 
        double[] referenciaPromptVerdad, 
        double frecuenciaAngularRespiracion)
    {
        Prompt = prompt;

        var admitancia = GetAdmitancia(referenciaPromptVerdad);        
        var palabra = new Palabra(
            Prompt,
            _verdad,
            frecuenciaAngularRespiracion,
            0.0,
            admitancia);
        NombrePromt = new Nombre(
            prompt,
            _verdad,
            admitancia);
        var designacion = NombrePromt.Mostrarse(palabra);
        var apariencia = designacion.Aparecer(Complex.One, 0.01, _verdad);
        return this;
    }    

    public ContextBuilder ConRespuesta(
        string respuesta, 
        double[] referenciaRespuestaPrompt, 
        double frecuenciaAngularRespiracion,
        double tiempoRespuesta)
    {
        if (NombrePromt == null || Prompt == null)
        {
            throw new InvalidOperationException("Debe configurar el prompt antes de la respuesta.");
        }

        Respuesta = respuesta;
        var admitancia = GetAdmitancia(referenciaRespuestaPrompt);
        NombreRespuesta = new Nombre(
            respuesta,
            _verdad,
            admitancia);

        return this;
    }

    private Func<double, Complex> GetAdmitancia(double[] referencia) => t =>
    {
        var indice = (int)t;
        if (indice >= 0 && indice < referencia.Length)
        {
            return referencia[indice];
        }

        return 0;
    };
}
