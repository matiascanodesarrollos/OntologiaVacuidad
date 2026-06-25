using System.Numerics;
using Xunit.Abstractions;

namespace DomainLogic;

public class DetectorAlucinacionIA
{
    private readonly string _verdad;    
    public Evaluacion Prompt { get; private set; }
    public Evaluacion Respuesta { get; private set; }
    private List<Func<bool>> Evaluaciones = new List<Func<bool>>();
    private ITestOutputHelper _output;

    public DetectorAlucinacionIA(string verdad)
    {
        _verdad = verdad;
    }

    public DetectorAlucinacionIA ConPrompt(
        string prompt, 
        Func<double, Complex> admitancia,
        Complex z)
    {
        var palabra = new Palabra(
            texto: prompt,
            contexto: _verdad,
            frecuenciaAngularRespiracion: 20,
            admitancia: admitancia);
        Prompt = new Evaluacion
        {
            Texto = prompt,
            Palabra = palabra,
            Apariencia = palabra.Efecto.Esencia,
            AparienciaDesignacion = palabra
                .Efecto
                .Mostrarse(palabra),
            AparienciaContextual = palabra
                .Efecto
                .Esencia
                .Esencia
                .Aparecer(z, prompt)
        };

        return this;
    }  

    public DetectorAlucinacionIA ConRespuesta(
        string respuesta, 
        Func<double, Complex> admitancia,
        Complex z)
    {
        if (Prompt == null)
        {
            throw new InvalidOperationException("Debe configurar el prompt antes de la respuesta.");
        }

        var palabra = new Palabra(
            texto: respuesta,
            contexto: Prompt.Texto,
            frecuenciaAngularRespiracion: 500,
            admitancia: admitancia);
        Respuesta = new Evaluacion
        {
            Texto = respuesta,
            Palabra = palabra,
            Apariencia = palabra.Efecto.Esencia,
            AparienciaDesignacion = palabra
                .Efecto
                .Mostrarse(palabra),
            AparienciaContextual = palabra
                .Efecto
                .Esencia
                .Esencia
                .Aparecer(z, respuesta)
        };
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
            var frecuenciaPrompt = Helper.ObtenerFrecuenciaDominante(Prompt.Apariencia.Funcion);
            var frecuenciaRespuesta = Helper.ObtenerFrecuenciaDominante(Respuesta.Apariencia.Funcion);
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
            var tiempoFinalPrompt = Helper.ObtenerTiempoFinal(tau => Prompt.Palabra.Funcion(tau, 0), tiempoMaximo, umbral);
            var tiempoFinalRespuesta = Helper.ObtenerTiempoFinal(tau => Respuesta.Palabra.Funcion(tau, 0), tiempoMaximo, umbral);
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

    public class Evaluacion
    {
        public string Texto { get; set; }
        public Palabra Palabra { get; set; }
        public Apariencia Apariencia { get; set;}
        public Apariencia AparienciaDesignacion { get; set;}
        public Apariencia AparienciaContextual { get; set;}

    }
}
