using System.Numerics;
using DomainLogic;
using FluentAssertions;
using Xunit.Abstractions;
namespace Models.Tests;

public class AITests
{
    private readonly AIDiagnostics _diagnostics = new AIDiagnostics();
    private readonly ITestOutputHelper _output;

    public AITests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Modelo_ConPreguntaLargaRespuestaMuyCorta_Alucina()
    {
        //Arrange
        var prompt = "Estoy muy emocionado con esto de charlar con una IA, siento que puedo encontrar cualquier cosa. Ahora decime ¿Cuál es la capital de Francia?";        
        var respuesta = "París";
        var _helper = new DetectorAlucinacionIA(
            prompt: prompt,
            respuesta: respuesta,
            admitancia: t => 
                    Complex.Exp(5 * t) * Complex.FromPolarCoordinates(20, 300 * t)
                    + Complex.Exp(1 * t) * Complex.FromPolarCoordinates(1, 200 * t)
                    + Complex.Exp(4 * t) * Complex.FromPolarCoordinates(3, 100 * t),
            fourierPrompt: new Dictionary<double, Complex>()
            {
                { 100, new Complex(30, 10) },
                { 200, new Complex(20, 40) },
                { 300, new Complex(2, 30) },
            },
            fourierRespuesta: new Dictionary<double, Complex>()
            {
                { 100, new Complex(2, 2) },
            },
            0
        );
        var evaluador = _helper
            .ConLogger(_output)
            .AmplitudCercana(20)
            .PortadorasDebenArmonizar(2, 1);

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
            respuesta: respuesta,
            admitancia: t => 
                    Complex.Exp(5 * t) * Complex.FromPolarCoordinates(20, 300 * t)
                    + Complex.Exp(1 * t) * Complex.FromPolarCoordinates(1, 200 * t)
                    + Complex.Exp(4 * t) * Complex.FromPolarCoordinates(3, 100 * t),
            fourierPrompt: new Dictionary<double, Complex>()
            {
                { 100, new Complex(30, 10) },
                { 200, new Complex(20, 40) },
                { 300, new Complex(2, 30) },
            },
            fourierRespuesta: new Dictionary<double, Complex>()
            {
                { 100, new Complex(30, 10) },
                { 200, new Complex(20, 40) },
                { 300, new Complex(2, 30) },
            },
            0
        );
        var evaluador = _helper
            .ConLogger(_output)
            .AmplitudCercana(20)
            .PortadorasDebenArmonizar(2, 1);

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
            respuesta: respuesta,
            admitancia: t => 
                    + Complex.Exp(4 * t) * Complex.FromPolarCoordinates(3, 100 * t),
            fourierPrompt: new Dictionary<double, Complex>()
            {
                { 100, new Complex(30, 10) },
            },
            fourierRespuesta: new Dictionary<double, Complex>()
            {
                { 100, new Complex(2, 5) },
            },
            0
        );
        var evaluador = _helper
            .ConLogger(_output)
            .AmplitudCercana(20)
            .PortadorasDebenArmonizar(2, 1);

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
            respuesta: respuesta,
            admitancia: t => Complex.Exp(4 * t) * Complex.FromPolarCoordinates(3, 100 * t),
            fourierPrompt: new Dictionary<double, Complex>()
            {
                { 100, new Complex(10, 10) },
            },
            fourierRespuesta: new Dictionary<double, Complex>()
            {
                { 100, new Complex(30, 10) },
                { 200, new Complex(20, 40) },
                { 300, new Complex(2, 30) },
            },
            0
        );
        var evaluador = _helper
            .ConLogger(_output)
            .AmplitudCercana(20)
            .PortadorasDebenArmonizar(2, 1);

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
            respuesta: respuesta,
            admitancia: t => Complex.Exp(4 * t) * Complex.FromPolarCoordinates(3, 100 * t),
            fourierPrompt: new Dictionary<double, Complex>()
            {
                { 100, new Complex(10, 10) },
            },
            fourierRespuesta: new Dictionary<double, Complex>()
            {
                { 100, new Complex(0, 0) },
            },
            0
        );
        var evaluador = _helper
            .ConLogger(_output)
            .AmplitudCercana(20)
            .PortadorasDebenArmonizar(2, 1);

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
            respuesta: respuesta,
            admitancia: t => 
                    Complex.Exp(5 * t) * Complex.FromPolarCoordinates(20, 300 * t)
                    + Complex.Exp(1 * t) * Complex.FromPolarCoordinates(1, 200 * t)
                    + Complex.Exp(4 * t) * Complex.FromPolarCoordinates(3, 100 * t),
            fourierPrompt: new Dictionary<double, Complex>()
            {
                { 100, new Complex(30, 10) },
                { 200, new Complex(20, 40) },
                { 300, new Complex(2, 30) },
            },
            fourierRespuesta: new Dictionary<double, Complex>()
            {
                { 100, new Complex(0, 0) },
                { 200, new Complex(20, 40) },
                { 300, new Complex(2, 30) },
            },
            0
        );
        var evaluador = _helper
            .ConLogger(_output)
            .AmplitudCercana(20)
            .PortadorasDebenArmonizar(2, 1);

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
