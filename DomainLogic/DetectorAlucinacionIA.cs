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
    private readonly double _frecuenciaRespiracionPrompt;
    private readonly double _frecuenciaRespiracionRespuesta;

    public DetectorAlucinacionIA(
        string prompt,  
        double frecuenciaRespiracionPrompt,       
        string respuesta,
        double frecuenciaRespiracionRespuesta,
        Func<double, Complex> admitancia,
        Dictionary<Complex, Complex> interpretacion,
        Func<double, Complex> ventanaRespuesta,
        double sigma,
        double x,
        Func<double, double, Complex> funcionTransferencia)
    {
        _frecuenciaRespiracionPrompt = frecuenciaRespiracionPrompt;
        _frecuenciaRespiracionRespuesta = frecuenciaRespiracionRespuesta;
        var palabraPrompt = new Palabra(
            texto: prompt,
            frecuenciaAngular: frecuenciaRespiracionPrompt,
            admitancia: admitancia
        );
        var objeto = interpretacion.ToDictionary(
            entrada => new KeyValuePair<double, KeyValuePair<double, double>>(
                entrada.Key.Imaginary,
                new KeyValuePair<double, double>(
                    entrada.Key.Real,
                    entrada.Key.Imaginary)
            ),
            entrada => entrada.Value);
        var nombre = new Nombre(
            sustantivo: respuesta,
            objeto: objeto,
            contexto: palabraPrompt,
            funcionTransferencia: funcionTransferencia
        );
        Designacion = new Designacion(
            esencia: palabraPrompt,
            naturaleza: nombre,
            ventana: ventanaRespuesta,
            sigma: sigma,
            x: x);

        Prompt = nombre.Esencia;
        Respuesta = Designacion.Efecto;
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
            var frecuenciaPrompt = (int) _frecuenciaRespiracionPrompt;
            var frecuenciaRespuesta = (int) _frecuenciaRespiracionRespuesta;
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

    public DetectorAlucinacionIA AmplitudCercana(
        double tolerancia)
    {
        AgregarEvaluacion(() => {
            var amplitudPromt = Prompt.Funcion(1, 0).Magnitude;
            var amplitudRespuesta = Respuesta.Funcion(1, 0).Magnitude;
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
