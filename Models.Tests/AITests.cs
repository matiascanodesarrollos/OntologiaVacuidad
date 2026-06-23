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
                    3 * Complex.Exp(new Complex(-0.8, 300) * t) 
                    + 10 * Complex.Exp(new Complex(-0.09, 200) * t) 
                    + 5 * Complex.Exp(new Complex(-0.01, 100) * t), 
                400)            
            .ConRespuesta(
                respuesta, 
                t => 
                    2 * Complex.Exp(new Complex(-5, 100) * t), 
                100)            
            .DebenDecaerEnMomentosCercanos(1000, 0.02, 20)
            .PortadorasDebenArmonizar(2, 1)
            .ConFiltroPrompt(400, respuesta)
            .ConFiltroRespuesta(100, respuesta);

        //Act
        var alucina = _helper.Alucina();

        //Assert
        if(alucina != true)
        {
            var carpetaDiagnostico = _diagnostics.GenerarDiagnosticos(evaluador);
            _output.WriteLine($"Diagnostico={carpetaDiagnostico}");
        }
        alucina.Should().BeTrue();
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
                    6 * Complex.Exp(new Complex(-0.8, 300) * t) 
                    + 10 * Complex.Exp(new Complex(-0.09, 200) * t) 
                    + 5 * Complex.Exp(new Complex(-0.01, 100) * t), 
                400)
            .ConRespuesta(
                respuesta, 
                t => 
                    6 * Complex.Exp(new Complex(-0.8, 300) * t) 
                    + 20 * Complex.Exp(new Complex(-0.01, 200) * t)
                    + 5 * Complex.Exp(new Complex(-0.01, 100) * t), 
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
        alucina.Should().BeFalse();
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
                    5 * Complex.Exp(new Complex(-0.01, 100) * t), 
                100)
            .ConRespuesta(
                respuesta, 
                t => 
                    5 * Complex.Exp(new Complex(-0.01, 100) * t), 
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
        alucina.Should().BeFalse();
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
                    5 * Complex.Exp(new Complex(-0.01, 100) * t), 
                100)
            .ConRespuesta(
                respuesta, 
                t => 
                    6 * Complex.Exp(new Complex(-0.8, 300) * t) 
                    + 5 * Complex.Exp(new Complex(-0.7, 200) * t)
                    + 5 * Complex.Exp(new Complex(-0.01, 100) * t), 
                400)
            .DebenDecaerEnMomentosCercanos(1000, 0.02, 20)
            .PortadorasDebenArmonizar(2, 1)
            .ConFiltroPrompt(100, respuesta)
            .ConFiltroRespuesta(400, respuesta);

        //Act
        var alucina = _helper.Alucina();

        //Assert
        if(alucina != true)
        {
            var carpetaDiagnostico = _diagnostics.GenerarDiagnosticos(evaluador);
            _output.WriteLine($"Diagnostico={carpetaDiagnostico}");
        }
        alucina.Should().BeTrue();
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
                    5 * Complex.Exp(new Complex(-0.01, 100) * t), 
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
        alucina.Should().BeTrue();
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
                    5 * Complex.Exp(new Complex(-0.01, 100) * t), 
                100)
            .ConRespuesta(
                respuesta, 
                t => 
                    6 * Complex.Exp(new Complex(-0.8, 300) * t) 
                    + 5 * Complex.Exp(new Complex(-0.7, 200) * t)
                    + 5 * Complex.Exp(new Complex(-0.01, 100) * t), 
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
        alucina.Should().BeTrue();
    }
    
}
