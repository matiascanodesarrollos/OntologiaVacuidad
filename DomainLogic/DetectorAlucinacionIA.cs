using System.Numerics;
using Xunit.Abstractions;

namespace DomainLogic;

public class DetectorAlucinacionIA
{
    public Apariencia Prompt { get; }
    public Apariencia Respuesta { get; }
    public Designacion Designacion { get; }
    private List<Func<bool>> Evaluaciones = new List<Func<bool>>();
    private ITestOutputHelper? _output;

    public DetectorAlucinacionIA(
        string prompt,  
        double frecuenciaRespiracionPrompt,       
        string respuesta,
        double frecuenciaRespiracionRespuesta,
        Func<double, Complex> admitancia,
        Dictionary<KeyValuePair<Complex, double>, Complex> interpretacion,
        Func<double, Complex> ventanaRespuesta)
    {
        var palabraPrompt = new Palabra(
            texto: prompt,
            frecuenciaAngular: frecuenciaRespiracionPrompt,
            admitancia: admitancia
        );
        var nombre = new Nombre(
            texto: respuesta,
            esferaAdmitancia: interpretacion,
            esencia: palabraPrompt);
        Designacion = new Designacion(
            naturaleza: nombre,
            esencia: palabraPrompt,
            ventana: ventanaRespuesta);

        Prompt = nombre.Esencia;
        var palabraRespuesta = Designacion
            .Aparecer(frecuenciaRespiracionRespuesta);
        Respuesta = new Apariencia(palabraRespuesta, nombre);
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
            var amplitudPromt = Prompt.Fasor.Magnitude;
            var amplitudRespuesta = Respuesta.Fasor.Magnitude;
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
