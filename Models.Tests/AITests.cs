using FluentAssertions;
using System.Numerics;
using static Models.Tests.AITestHelpers;
namespace Models.Tests;

public class AITests
{
    [Theory]
    [InlineData(Math.PI / 2, 4.0)]
    [InlineData(Math.PI / 2, 5.0)]
    public void Modelo_ConRespuestaCorrectaMuyCorta_NoAlucina(double toleranciaDefase, double factorUmbralMagnitud)
    {
        //Arrage
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        var energia = prompt.Length;
        // Caracter por caracter se indica a que parte de la verdad se refiere cada caracter del prompt (0: no relevante, 1: se refiere a Francia, 2: se refiere a capital, 3: se refiere a París).
        var referenciaPromptVerdad = new double[] { 0, 2, 2, 2, 2, 0, 2, 2, 0, 2, 2, 0, 2, 2, 2, 2, 2, 2, 2, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0 };
        var respuesta = "París";
        var referenciaRespuestaPrompt = new double[] { 3, 3, 3, 3, 3 };

        //Act & Assert
        var (alucina, detalleFallo) = EvaluarAlucina(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            toleranciaDefase, 
            factorUmbralMagnitud,
            energia,
            esperado: false);
        alucina.Should().BeFalse(detalleFallo);
    }

    [Theory]
    [InlineData(Math.PI / 2, 4.0)]
    [InlineData(Math.PI / 2, 5.0)]
    public void Modelo_ConRespuestaCorrectaConRellenoMargenBajo_AlucinaPorMagnitud(double toleranciaDefase, double factorUmbralMagnitud)
    {
        //Arrage
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        var energia = prompt.Length;
        // Caracter por caracter se indica a que parte de la verdad se refiere cada caracter del prompt (0: no relevante, 1: se refiere a Francia, 2: se refiere a capital, 3: se refiere a París).
        var referenciaPromptVerdad = new double[] { 0, 2, 2, 2, 2, 0, 2, 2, 0, 2, 2, 0, 2, 2, 2, 2, 2, 2, 2, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0 };
        var respuesta = "La capital de Francia es París.";
        var referenciaRespuestaPrompt = new double[] { 1,1, 2,2,2,2,2,2,2, 1,1, 1,1,1,1,1,1, 2,2, 3,3,3,3,3 };

        //Act & Assert
        var (alucina, detalleFallo) = EvaluarAlucina(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            toleranciaDefase, 
            factorUmbralMagnitud,
            energia,
            esperado: true);
        alucina.Should().BeTrue(detalleFallo);
    }

    [Theory]
    [InlineData(Math.PI / 2, 5.0)]
    [InlineData(Math.PI / 2, 10.0)]
    [InlineData(Math.PI / 2, 15.0)]
    [InlineData(Math.PI / 2, 20.0)]
    [InlineData(Math.PI / 2, 29.0)]
    public void Modelo_ConRespuestaCorrectaConRellenoMargenMedio_NoAlucina(double toleranciaDefase, double factorUmbralMagnitud)
    {
        //Arrage
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        var energia = prompt.Length;
        // Caracter por caracter se indica a que parte de la verdad se refiere cada caracter del prompt (0: no relevante, 1: se refiere a Francia, 2: se refiere a capital, 3: se refiere a París).
        var referenciaPromptVerdad = new double[] { 0, 2, 2, 2, 2, 0, 2, 2, 0, 2, 2, 0, 2, 2, 2, 2, 2, 2, 2, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0 };
        var respuesta = "La capital de Francia es París.";
        var referenciaRespuestaPrompt = new double[] { 1,1, 2,2,2,2,2,2,2, 1,1, 1,1,1,1,1,1, 2,2, 3,3,3,3,3 };

        //Act & Assert
        var (alucina, detalleFallo) = EvaluarAlucina(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            toleranciaDefase, 
            factorUmbralMagnitud,
            energia,
            esperado: true);
        alucina.Should().BeTrue(detalleFallo);
    }

    [Theory]
    [InlineData(Math.PI / 2, 5.0)]
    [InlineData(Math.PI / 2, 10.0)]
    [InlineData(Math.PI / 2, 20.0)]
    [InlineData(Math.PI / 2, 30.0)]
    public void Modelo_ConMuchoRelleno_AlucinaMagnitud(double toleranciaDefase, double factorUmbralMagnitud)
    {
        //Arrage
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        var energia = prompt.Length;
        // Caracter por caracter se indica a que parte de la verdad se refiere cada caracter del prompt (0: no relevante, 1: se refiere a Francia, 2: se refiere a capital, 3: se refiere a París).
        var referenciaPromptVerdad = new double[] { 0, 2, 2, 2, 2, 0, 2, 2, 0, 2, 2, 0, 2, 2, 2, 2, 2, 2, 2, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0 };
        var respuesta = "Hay muchas repuestas posibles correctas, algunos dicen que es Lyon pero la verdadera capital de Francia es París.";
        var referenciaRespuestaPrompt = new double[] { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

        //Act & Assert
        var (alucina, detalleFallo) = EvaluarAlucina(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            toleranciaDefase, 
            factorUmbralMagnitud,
            energia,
            esperado: true);
        alucina.Should().BeTrue(detalleFallo);
    }

}
