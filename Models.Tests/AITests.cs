using System.Numerics;
using DomainLogic;
using FluentAssertions;
namespace Models.Tests;

public class AITests
{
    private readonly DetectorAlucinacionIA _helper = new DetectorAlucinacionIA("Francia:capital:París");
    private readonly AIDiagnostics _diagnostics = new AIDiagnostics();

    [Fact]
    public void Modelo_ConPreguntaLargaRespuestaMuyCorta_Alucina()
    {
        //Arrange
        var prompt = "Estoy muy emocionado con esto de charlar con una IA, siento que puedo encontrar cualquier cosa. Ahora decime ¿Cuál es la capital de Francia?";        
        var respuesta = "París";
        var evaluador = _helper
            .ConPrompt(prompt, t => Complex.Zero, 0)
            .ConRespuesta(respuesta, t => Complex.Zero, 0, 0)
            .AgregarEvaluacion(() => true);

        //Act
        var alucina = _helper.Alucina();

        //Assert
        if(alucina != true)
        {
            var carpetaDiagnostico = _diagnostics.GenerarDiagnosticos(evaluador);
            Console.WriteLine($"Diagnostico={carpetaDiagnostico}");
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
            .ConPrompt(prompt, t => Complex.Zero, 0)
            .ConRespuesta(respuesta, t => Complex.Zero, 0, 0)
            .AgregarEvaluacion(() => false);

        //Act
        var alucina = _helper.Alucina();

        //Assert
        if(alucina != false)
        {
            var carpetaDiagnostico = _diagnostics.GenerarDiagnosticos(evaluador);
            Console.WriteLine($"Diagnostico={carpetaDiagnostico}");
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
            .ConPrompt(prompt, t => Complex.Zero, 0)
            .ConRespuesta(respuesta, t => Complex.Zero, 0, 0)
            .AgregarEvaluacion(() => false);

        //Act
        var alucina = _helper.Alucina();

        //Assert
        if(alucina != false)
        {
            var carpetaDiagnostico = _diagnostics.GenerarDiagnosticos(evaluador);
            Console.WriteLine($"Diagnostico={carpetaDiagnostico}");
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
            .ConPrompt(prompt, t => Complex.Zero, 0)
            .ConRespuesta(respuesta, t => Complex.Zero, 0, 0)
            .AgregarEvaluacion(() => true);

        //Act
        var alucina = _helper.Alucina();

        //Assert
        if(alucina != true)
        {
            var carpetaDiagnostico = _diagnostics.GenerarDiagnosticos(evaluador);
            Console.WriteLine($"Diagnostico={carpetaDiagnostico}");
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
            .ConPrompt(prompt, t => Complex.Zero, 0)
            .ConRespuesta(respuesta, t => Complex.Zero, 0, 0)
            .AgregarEvaluacion(() => true);

        //Act
        var alucina = _helper.Alucina();

        //Assert
        if(alucina != true)
        {
            var carpetaDiagnostico = _diagnostics.GenerarDiagnosticos(evaluador);
            Console.WriteLine($"Diagnostico={carpetaDiagnostico}");
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
            .ConPrompt(prompt, t => Complex.Zero, 0)
            .ConRespuesta(respuesta, t => Complex.Zero, 0, 0)
            .AgregarEvaluacion(() => true);

        //Act
        var alucina = _helper.Alucina();

        //Assert
        if(alucina != true)
        {
            var carpetaDiagnostico = _diagnostics.GenerarDiagnosticos(evaluador);
            Console.WriteLine($"Diagnostico={carpetaDiagnostico}");
        }
        alucina.Should().BeTrue();
    }
    
}
