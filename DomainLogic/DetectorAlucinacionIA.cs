using System.Numerics;
using Xunit.Abstractions;

namespace DomainLogic;

public class DetectorAlucinacionIA
{
    private readonly string _verdad;    
    public Palabra Prompt { get; private set; }
    private double _frecuenciaAngularRespiracionPrompt;
    public Palabra Respuesta { get; private set; }
    private List<Func<bool>> Evaluaciones = new List<Func<bool>>();
    private ITestOutputHelper _output;

    public DetectorAlucinacionIA(string verdad)
    {
        _verdad = verdad;
    }

    public DetectorAlucinacionIA ConPrompt(
        string prompt, 
        Func<double, Complex> admitancia, 
        double frecuenciaAngularRespiracion)
    {
        _frecuenciaAngularRespiracionPrompt = frecuenciaAngularRespiracion;
        Prompt = new Palabra(
            prompt,
            _verdad,
            frecuenciaAngularRespiracion,
            admitancia);
        return this;
    }  

    public DetectorAlucinacionIA ConRespuesta(
        string respuesta, 
        Func<double, Complex> admitancia, 
        double frecuenciaAngularRespiracion)
    {
        if (Prompt == null)
        {
            throw new InvalidOperationException("Debe configurar el prompt antes de la respuesta.");
        }

        Respuesta = new Palabra(
            respuesta,
            Prompt.Texto,
            frecuenciaAngularRespiracion,
            admitancia);
        return this;
    }    

    public DetectorAlucinacionIA ConLogger(ITestOutputHelper output)
    {
        _output = output;
        return this;
    }

    public DetectorAlucinacionIA AgregarEvaluacion(Func<bool> evaluacion)
    {        
        Evaluaciones.Add(evaluacion);
        return this;
    }

    public DetectorAlucinacionIA PortadorasDebenArmonizar(double tolerancia, uint maximoArmonicos)
    {
        AgregarEvaluacion(() => {
            var aparienciaPrompt = Prompt as Apariencia;
            var frecuenciaPrompt = Helper.ObtenerFrecuenciaDominante(aparienciaPrompt.Funcion);
            var aparienciaRespuesta = Respuesta as Apariencia;            
            var frecuenciaRespuesta = Helper.ObtenerFrecuenciaDominante(aparienciaRespuesta.Funcion);
            if (_output != null)
            {
                _output.WriteLine($"FrecuenciaPrompt={frecuenciaPrompt}, frecuenciaRespuesta={frecuenciaRespuesta}, tolerancia={tolerancia}, maximoArmonicos={maximoArmonicos}.");
            } 

            var minimoFrecuencia = Math.Min(frecuenciaPrompt, frecuenciaRespuesta);
            var maximoFrecuencia = Math.Max(frecuenciaPrompt, frecuenciaRespuesta);
            for(var i = 1; i <= maximoArmonicos; i++)
            {
                if (Math.Abs(minimoFrecuencia * i - maximoFrecuencia) <= Math.Abs(tolerancia))
                {
                    return false;
                }
            }                       
            return true;
        });
        return this;
    }

    public DetectorAlucinacionIA DebenDecaerEnMomentosCercanos(
        double tiempoMaximo, 
        double umbral,
        double tolerancia)
    {
        AgregarEvaluacion(() => {
            var tiempoFinalPrompt = Helper.ObtenerTiempoFinal(Prompt.Funcion, tiempoMaximo, umbral);
            var tiempoFinalRespuesta = Helper.ObtenerTiempoFinal(Respuesta.Funcion, tiempoMaximo, umbral);
            if (_output != null)
            {
                _output.WriteLine($"TiempoFinalPrompt={tiempoFinalPrompt}, TiempoFinalRespuesta={tiempoFinalRespuesta}, tiempoMaximo={tiempoMaximo}, umbral={umbral}.");
            } 
            return Math.Abs(tiempoFinalPrompt - tiempoFinalRespuesta) > tolerancia;
        });
        return this;
    }
    

    public bool Alucina()
    {
        if (Prompt == null 
            || Respuesta == null)
        {
            throw new InvalidOperationException("Debe configurar el prompt y la respuesta antes de evaluar.");
        }

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
