using System.Numerics;
using Xunit.Abstractions;

namespace DomainLogic;

public class DetectorAlucinacionIA
{
    public Evaluacion Resultado { get; private set; }
    private List<Func<bool>> Evaluaciones = new List<Func<bool>>();
    private ITestOutputHelper _output;

    public DetectorAlucinacionIA()
    {
    }

    public DetectorAlucinacionIA ConDatos(
        string prompt, 
        string respuesta,
        Dictionary<Complex, Complex> admitancia,
        Complex ondaRespuesta)
    {
        var designacion = new Designacion(
            texto: respuesta,
            contexto: prompt,
            frecuenciaAngularRespiracion: ondaRespuesta.Phase,
            transformadaZ: admitancia);
        var palabraRespuesta = designacion
            .Aparecer(ondaRespuesta, respuesta);
        var aparienciaContextual = palabraRespuesta
            .Esencia
            .Mostrarse(palabraRespuesta);
        Resultado = new Evaluacion
        {
            Texto = prompt,
            PalabraRespuesta = palabraRespuesta,
            AparienciaRespuesta = palabraRespuesta,
            AparienciaContextual = aparienciaContextual,
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
            var frecuenciaPrompt = Helper.ObtenerFrecuenciaDominante(Resultado.AparienciaRespuesta.Funcion);
            var frecuenciaRespuesta = Helper.ObtenerFrecuenciaDominante(Resultado.AparienciaContextual.Funcion);
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
            var tiempoFinalPrompt = Helper.ObtenerTiempoFinal(tau => Resultado.PalabraRespuesta.Funcion(tau, 0), tiempoMaximo, umbral);
            var tiempoFinalRespuesta = Helper.ObtenerTiempoFinal(Resultado.AparienciaContextual.Funcion, tiempoMaximo, umbral);
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
        if (Resultado == null)
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
        public Palabra PalabraRespuesta { get; set; }
        public Apariencia AparienciaRespuesta { get; set;}
        public Apariencia AparienciaContextual { get; set;}

    }
}
