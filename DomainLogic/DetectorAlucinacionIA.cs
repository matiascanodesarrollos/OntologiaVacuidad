using System.Numerics;

namespace DomainLogic;

public class DetectorAlucinacionIA
{
    private readonly string _verdad;
    public string Prompt { get; private set; }
    public Nombre NombrePromt;
    public string Respuesta { get; private set; }
    public Nombre NombreRespuesta;
    private List<Func<bool>> Evaluaciones = new List<Func<bool>>();

    public DetectorAlucinacionIA(string verdad)
    {
        _verdad = verdad;
    }

    public DetectorAlucinacionIA ConPrompt(
        string prompt, 
        Func<double, Complex> admitancia, 
        double frecuenciaAngularRespiracion)
    {
        Prompt = prompt;
     
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
        var apariencia = NombrePromt.Mostrarse(palabra);
        var respuesta = NombrePromt.Esencia.Esencia.Aparecer(Complex.One, 0.01, _verdad);
        return this;
    }    

    public DetectorAlucinacionIA ConRespuesta(
        string respuesta, 
        Func<double, Complex> admitancia, 
        double frecuenciaAngularRespiracion,
        double tiempoRespuesta)
    {
        if (NombrePromt == null || Prompt == null)
        {
            throw new InvalidOperationException("Debe configurar el prompt antes de la respuesta.");
        }

        Respuesta = respuesta;
        NombreRespuesta = new Nombre(
            respuesta,
            _verdad,
            admitancia);

        return this;
    }

    public DetectorAlucinacionIA AgregarEvaluacion(Func<bool> evaluacion)
    {
        Evaluaciones.Add(evaluacion);
        return this;
    }

    public bool Alucina()
    {
        var alucina = false;
        foreach (var evaluacion in Evaluaciones)
        {
            if (evaluacion())
            {
                alucina = true;
                break;
            }
        }
        
        return alucina;
    }
}
