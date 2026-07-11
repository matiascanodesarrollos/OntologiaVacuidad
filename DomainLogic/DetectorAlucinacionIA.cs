using System.Numerics;
using Xunit.Abstractions;

namespace DomainLogic;

public class DetectorAlucinacionIA
{
    public Apariencia Prompt { get; }
    public Apariencia Respuesta { get; }
    private List<Func<bool>> Evaluaciones = new List<Func<bool>>();
    private ITestOutputHelper _output;

    public DetectorAlucinacionIA(
        string prompt,         
        string respuesta,
        Func<double, Complex> admitancia,
        Dictionary<double, Complex> fourierPrompt,
        Dictionary<double, Complex> fourierRespuesta,
        double sigma)
    {
        var palabra = new Palabra(
            texto: prompt,
            contexto: string.Empty,
            frecuenciaAngular: fourierPrompt.Keys.Sum(),
            admitancia: admitancia,
            fourierPrompt
        );
        Prompt = palabra;
        var nombreRespuesta = new Nombre(
            texto: respuesta,
            contexto: prompt,
            fourier: fourierRespuesta,
            Prompt);
        Prompt.Esencia = nombreRespuesta;
        Respuesta = new Designacion(
            naturaleza: nombreRespuesta,
            causa: palabra,
            sigma)
            .Mostrarse(nombreRespuesta.Aparecer(), respuesta, prompt);        
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
            var frecuenciaPrompt = (int) Prompt.FrecuenciaAngular;
            var frecuenciaRespuesta = (int) Respuesta.FrecuenciaAngular;
            if (_output != null)
            {
                _output.WriteLine($"FrecuenciaPrompt={frecuenciaPrompt}, frecuenciaRespuesta={Respuesta.FrecuenciaAngular}, tolerancia={tolerancia}, maximoArmonicos={maximoArmonicos}.");
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

    public DetectorAlucinacionIA AmplitudCercana(
        double tolerancia)
    {
        AgregarEvaluacion(() => {
            var amplitudPromt = Prompt.Fasor.Value.Magnitude;
            var amplitudRespuesta = Respuesta.Fasor.Value.Magnitude;
            if (_output != null)
            {
                _output.WriteLine($"AmplitudPrompt={amplitudPromt}, AmplitudRespuesta={amplitudRespuesta}.");
            } 
            return Math.Abs(amplitudPromt - amplitudRespuesta) > tolerancia;
        });
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
