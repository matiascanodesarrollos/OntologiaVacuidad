using System.Numerics;
using Xunit.Abstractions;

namespace DomainLogic;

public class DetectorAlucinacionIA
{
    private readonly string _verdad;    
    public Evaluador Prompt { get; private set; }
    public Evaluador Respuesta { get; private set; }
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
        var palabra = new Palabra(
            prompt,
            _verdad,
            frecuenciaAngularRespiracion,
            admitancia);
        var nombre = new Nombre(
            prompt,
            _verdad,
            admitancia);
        Prompt = new Evaluador()
        {
            Texto = prompt,
            Palabra = palabra,
            Nombre = nombre,
            Apariencia = nombre.Mostrarse(palabra),
        };
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

        var palabra = new Palabra(
            respuesta,
            _verdad,
            frecuenciaAngularRespiracion,
            admitancia);
        var nombre = new Nombre(
            respuesta,
            _verdad,
            admitancia);
        Respuesta = new Evaluador()
        {
            Texto = respuesta,
            Palabra = palabra,
            Nombre = nombre,
            Apariencia = nombre.Mostrarse(palabra),
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

    public DetectorAlucinacionIA ConFiltroPrompt(
        double frecuenciaAngularRespiracion, 
        string textoDeseado)
    {
        if (Prompt == null)
        {
            throw new InvalidOperationException("Debe configurar el prompt antes de aplicar el filtro.");
        }

        AgregarEvaluacion(() => {
            var esencia = Prompt
                .Apariencia
                .Esencia
                .Aparecer(frecuenciaAngularRespiracion, textoDeseado);
            return false;
        });
        return this;
    }

    public DetectorAlucinacionIA ConFiltroRespuesta(
        double frecuenciaAngularRespiracion, 
        string textoDeseado)
    {
        if (Respuesta == null)
        {
            throw new InvalidOperationException("Debe configurar la respuesta antes de aplicar el filtro.");
        }

        AgregarEvaluacion(() => {
            var esencia = Respuesta
                .Apariencia
                .Esencia
                .Aparecer(frecuenciaAngularRespiracion, textoDeseado);
            return false;
        });
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
            var tiempoFinalPrompt = Helper.ObtenerTiempoFinal(Prompt.Palabra.Funcion, 0.0, tiempoMaximo, umbral);
            var tiempoFinalRespuesta = Helper.ObtenerTiempoFinal(Respuesta.Palabra.Funcion, 1.0, tiempoMaximo, umbral);
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

    public class Evaluador
    {
        public string Texto { get; set; }
        public Palabra Palabra { get; set; }
        public Nombre Nombre { get; set; }
        public Apariencia Apariencia { get; set; }
    }
}
