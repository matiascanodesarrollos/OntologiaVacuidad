using System.Numerics;
using DomainLogic;
using FluentAssertions;
using Xunit.Abstractions;
namespace Models.Tests;

public class AITests
{
    private readonly DetectorAlucinacionIA _helper = new DetectorAlucinacionIA("Francia:capital:París");
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
        var evaluador = _helper
            .ConLogger(_output)
            .ConPrompt(
                prompt, 
                t => 
                    Complex.Exp(new Complex(0.5 * t, 300 * t)) 
                    + Complex.Exp(new Complex(0.01 * t, 200 * t))
                    + Complex.Exp(new Complex(0.4 * t, 100 * t)), 
                400)            
            .ConRespuesta(
                respuesta, 
                t => 
                    Complex.Exp(new Complex(0.1 * t, 100 * t)), 
                100)            
            .DebenDecaerEnMomentosCercanos(1000, 0.02, 20)
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
        var evaluador = _helper
            .ConLogger(_output)
            .ConPrompt(
                prompt, 
                t => 
                    Complex.Exp(new Complex(0.8 * t, 300 * t)) 
                    + Complex.Exp(new Complex(0.01 * t, 200 * t)) 
                    + Complex.Exp(new Complex(0.3 * t, 100 * t)), 
                400)
            .ConRespuesta(
                respuesta, 
                t => 
                    Complex.Exp(new Complex(0.8 * t, 300 * t)) 
                    + Complex.Exp(new Complex(0.01 * t, 200 * t))
                    + Complex.Exp(new Complex(0.3 * t, 100 * t)), 
                400)
            .DebenDecaerEnMomentosCercanos(1000, 0.02, 20)
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
        var evaluador = _helper
            .ConLogger(_output)
            .ConPrompt(
                prompt, 
                t => 
                    Complex.Exp(new Complex(0.1 * t, 100 * t)), 
                100)
            .ConRespuesta(
                respuesta, 
                t => 
                    Complex.Exp(new Complex(0.1 * t, 100 * t)), 
                100)
            .DebenDecaerEnMomentosCercanos(1000, 0.02, 20)
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
        var evaluador = _helper
            .ConLogger(_output)
            .ConPrompt(
                prompt, 
                t => 
                    Complex.Exp(new Complex(0.2 * t, 100 * t)), 
                100)
            .ConRespuesta(
                respuesta, 
                t => 
                    Complex.Exp(new Complex(0.8 * t, 100 * t)),
                100)
            .DebenDecaerEnMomentosCercanos(5000, 0.1, 5)
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
        var evaluador = _helper
            .ConLogger(_output)
            .ConPrompt(
                prompt, 
                t => 
                    Complex.Exp(new Complex(0.3 * t, 100 * t)), 
                100)
            .ConRespuesta(
                respuesta, 
                t => 0, 
                0)
            .DebenDecaerEnMomentosCercanos(1000, 0.02, 20)
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
        var evaluador = _helper
            .ConLogger(_output)
            .ConPrompt(
                prompt, 
                t => 
                    Complex.Exp(new Complex(0.2 * t, 100 * t)), 
                100)
            .ConRespuesta(
                respuesta, 
                t => 
                    Complex.Exp(new Complex(0.8 * t, 300 * t)) 
                    + Complex.Exp(new Complex(0.01 * t, 200 * t))
                    + Complex.Exp(new Complex(0.2 * t, 100 * t)), 
                400)
            .DebenDecaerEnMomentosCercanos(1000, 0.02, 20)
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
