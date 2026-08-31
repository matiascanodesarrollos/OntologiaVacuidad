using System.Numerics;
using DomainLogic;
using FluentAssertions;
using Xunit.Abstractions;
namespace Models.Tests;

public class AITests
{
    private const double ToleranciaAmplitud = 2;
    private const double ToleranciaPortadoras = 2;
    private const uint MaximoArmonicosPortadoras = 1;
    private const double FrecuenciaPrompt = 100;

    private readonly AIDiagnostics _diagnostics = new AIDiagnostics();
    private readonly ITestOutputHelper _output;

    public AITests(ITestOutputHelper output)
    {
        _output = output;
    }

    private static Dictionary<KeyValuePair<Complex, double>, Complex> CrearInterpretacion()
    {
        return new Dictionary<KeyValuePair<Complex, double>, Complex>
        {
            { new KeyValuePair<Complex, double>(Complex.Zero, FrecuenciaPrompt), Complex.One },
        };
    }

    private static Func<double, Complex> VentanaEscalar(double escala)
    {
        return _ => new Complex(escala, 0);
    }

    [Fact]
    public void Modelo_ConPreguntaLargaRespuestaMuyCorta_Alucina()
    {
        //Arrange
        var prompt = "Estoy muy emocionado con esto de charlar con una IA, siento que puedo encontrar cualquier cosa. Ahora decime ¿Cuál es la capital de Francia?";        
        var respuesta = "París";
        var _helper = new DetectorAlucinacionIA(
            prompt: prompt,
            frecuenciaRespiracionPrompt: FrecuenciaPrompt,
            respuesta: respuesta,
            admitancia: t => 
                    Complex.Exp(5 * t) * Complex.FromPolarCoordinates(40, 300 * t)
                    + Complex.Exp(1 * t) * Complex.FromPolarCoordinates(2, 200 * t)
                    + Complex.Exp(4 * t) * Complex.FromPolarCoordinates(6, 100 * t),
            interpretacion: CrearInterpretacion(),
            ventanaRespuesta: VentanaEscalar(0)
        );
        var evaluador = _helper
            .ConLogger(_output)
            .AmplitudCercana(ToleranciaAmplitud)
            .PortadorasDebenArmonizar(ToleranciaPortadoras, MaximoArmonicosPortadoras);

        //Act
        var alucina = _helper.Alucina();

        //Assert
        if(alucina != true)
        {
            var carpetaDiagnostico = _diagnostics.GenerarDiagnosticos(evaluador);
            _output.WriteLine($"Diagnostico={carpetaDiagnostico}");
        }
        alucina.Should().BeTrue("la respuesta con mucho relleno y desviación de la pregunta original.");
    }

    [Fact]
    public void Modelo_ConPreguntaLargaRespuestaAcorde_NoAlucina()
    {
        //Arrange
        var prompt = "Estoy muy emocionado con esto de charlar con una IA, siento que puedo encontrar cualquier cosa. Ahora decime ¿Cuál es la capital de Francia?";
        var respuesta = "Me alegro mucho, es una emoción común la que experimentas. Podes aprender sobre muchos temas con IA, aunque siempre es recomendable verificar datos sensibles. Con respecto a la capital de Francia, es París";
        var _helper = new DetectorAlucinacionIA(
            prompt: prompt,
            frecuenciaRespiracionPrompt: FrecuenciaPrompt,
            respuesta: respuesta,
            admitancia: t => 
                    Complex.Exp(-2 * t) * Complex.FromPolarCoordinates(0.02, 300 * t)
                    + Complex.Exp(-1 * t) * Complex.FromPolarCoordinates(0.01, 200 * t)
                    + Complex.Exp(-2 * t) * Complex.FromPolarCoordinates(0.02, 100 * t),
            interpretacion: CrearInterpretacion(),
            ventanaRespuesta: VentanaEscalar(1)
        );
        var evaluador = _helper
            .ConLogger(_output)
            .AmplitudCercana(ToleranciaAmplitud)
            .PortadorasDebenArmonizar(ToleranciaPortadoras, MaximoArmonicosPortadoras);

        //Act
        var alucina = _helper.Alucina();

        //Assert
        if(alucina != false)
        {
            var carpetaDiagnostico = _diagnostics.GenerarDiagnosticos(evaluador);
            _output.WriteLine($"Diagnostico={carpetaDiagnostico}");
        }
        alucina.Should().BeFalse("la respuesta concisa y correcta.");
    }

    [Fact]
    public void Modelo_ConPreguntaConcisaRespuestaConcisa_NoAlucina()
    {
        //Arrange
        var prompt = "¿Cuál es la capital de Francia?";
        var respuesta = "La capital de Francia es París.";
        var _helper = new DetectorAlucinacionIA(
            prompt: prompt,
            frecuenciaRespiracionPrompt: FrecuenciaPrompt,
            respuesta: respuesta,
            admitancia: t => 
                    Complex.Exp(-1 * t) * Complex.FromPolarCoordinates(0.03, 100 * t),
            interpretacion: CrearInterpretacion(),
            ventanaRespuesta: VentanaEscalar(1)
        );
        var evaluador = _helper
            .ConLogger(_output)
            .AmplitudCercana(ToleranciaAmplitud)
            .PortadorasDebenArmonizar(ToleranciaPortadoras, MaximoArmonicosPortadoras);

        //Act
        var alucina = _helper.Alucina();

        //Assert
        if(alucina != false)
        {
            var carpetaDiagnostico = _diagnostics.GenerarDiagnosticos(evaluador);
            _output.WriteLine($"Diagnostico={carpetaDiagnostico}");
        }
        alucina.Should().BeFalse("la respuesta concisa y correcta.");
    }

    [Fact]
    public void Modelo_ConPreguntaConcisaRespuestaMuchoRelleno_Alucina()
    {
        //Arrange
        var prompt = "¿Cuál es la capital de Francia?";
        var respuesta = "Hay muchas repuestas posibles correctas, algunos dicen que es Lyon pero la verdadera capital de Francia es París.";
        var _helper = new DetectorAlucinacionIA(
            prompt: prompt,
            frecuenciaRespiracionPrompt: FrecuenciaPrompt,
            respuesta: respuesta,
            admitancia: t => Complex.Exp(4 * t) * Complex.FromPolarCoordinates(3, 100 * t),
            interpretacion: CrearInterpretacion(),
            ventanaRespuesta: VentanaEscalar(0)
        );
        var evaluador = _helper
            .ConLogger(_output)
            .AmplitudCercana(ToleranciaAmplitud)
            .PortadorasDebenArmonizar(ToleranciaPortadoras, MaximoArmonicosPortadoras);

        //Act
        var alucina = _helper.Alucina();

        //Assert
        if(alucina != true)
        {
            var carpetaDiagnostico = _diagnostics.GenerarDiagnosticos(evaluador);
            _output.WriteLine($"Diagnostico={carpetaDiagnostico}");
        }
        alucina.Should().BeTrue("la respuesta con mucho relleno y desviación de la pregunta original.");
    }

    [Fact]
    public void Modelo_ConPreguntaConcisaRespuestaCortaFalsa_Alucina()
    {
        //Arrange
        var prompt = "¿Cuál es la capital de Francia?";
        var respuesta = "Lyon.";
        var _helper = new DetectorAlucinacionIA(
            prompt: prompt,
            frecuenciaRespiracionPrompt: FrecuenciaPrompt,
            respuesta: respuesta,
            admitancia: t => Complex.Exp(4 * t) * Complex.FromPolarCoordinates(3, 100 * t),
            interpretacion: CrearInterpretacion(),
            ventanaRespuesta: VentanaEscalar(0)
        );
        var evaluador = _helper
            .ConLogger(_output)
            .AmplitudCercana(ToleranciaAmplitud)
            .PortadorasDebenArmonizar(ToleranciaPortadoras, MaximoArmonicosPortadoras);

        //Act
        var alucina = _helper.Alucina();

        //Assert
        if(alucina != true)
        {
            var carpetaDiagnostico = _diagnostics.GenerarDiagnosticos(evaluador);
            _output.WriteLine($"Diagnostico={carpetaDiagnostico}");
        }
        alucina.Should().BeTrue("la respuesta corta y falsa.");
    }

    [Fact]
    public void Modelo_ConPreguntaConcisaRespuestaLargaFalsa_Alucina()
    {
        //Arrange
        var prompt = "¿Cuál es la capital de Francia?";
        var respuesta = "Me alegro mucho, es una emoción común la que experimentas. Podes aprender sobre muchos temas con IA, aunque siempre es recomendable verificar datos sensibles. Con respecto a la capital de Francia, es Lyon.";
        var _helper = new DetectorAlucinacionIA(
            prompt: prompt,
            frecuenciaRespiracionPrompt: FrecuenciaPrompt,
            respuesta: respuesta,
            admitancia: t => Complex.Exp(4 * t) * Complex.FromPolarCoordinates(50, 600 * t),
            interpretacion: CrearInterpretacion(),
            ventanaRespuesta: t => new Complex(1 + 0.5 * Math.Sin(10 * t), 0)
        );
        var evaluador = _helper
            .ConLogger(_output)
            .AmplitudCercana(ToleranciaAmplitud)
            .PortadorasDebenArmonizar(ToleranciaPortadoras, MaximoArmonicosPortadoras);

        //Act
        var alucina = _helper.Alucina();

        //Assert
        if(alucina != true)
        {
            var carpetaDiagnostico = _diagnostics.GenerarDiagnosticos(evaluador);
            _output.WriteLine($"Diagnostico={carpetaDiagnostico}");
        }
        alucina.Should().BeTrue("la respuesta larga y falsa.");
    }
    
}
