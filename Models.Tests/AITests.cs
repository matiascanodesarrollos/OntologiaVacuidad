using FluentAssertions;
namespace Models.Tests;

public class AITests
{
    private readonly AITestHelpers _helper = new AITestHelpers();

    [Theory]
    [InlineData(0.03, 15.0)]
    public void Modelo_ConRespuestaMuyCorta_NoAlucina(double umbralPlv, double factorUmbralMagnitud)
    {
        //Arrage
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        var energia = prompt.Length;
        // Caracter por caracter se indica a que parte de la verdad se refiere cada caracter del prompt (0: no relevante, 1: se refiere a Francia, 2: se refiere a capital, 3: se refiere a París).
        var referenciaPromptVerdad = new double[] { 0, 2,2,2,2, 0, 2,2, 0, 2,2, 0, 2,2,2,2,2,2,2, 0, 1,1, 0, 1,1,1,1,1,1,1, 0 };
        var respuesta = "París";
        var referenciaRespuestaPrompt = new double[] { 3,3,3,3,3 };

        //Act & Assert
        var (alucina, detalleFallo) = _helper.EvaluarAlucina(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            umbralPlv,
            factorUmbralMagnitud,
            energia,
            esperado: false);
        alucina.Should().BeFalse(detalleFallo);
    }

    [Theory]
    [InlineData(0.03, 15.0)]
    public void Modelo_ConRespuestaCorrectaConRellenoMargenBajo_NoAlucina(double umbralPlv, double factorUmbralMagnitud)
    {
        //Arrage
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        var energia = prompt.Length;
        // Caracter por caracter se indica a que parte de la verdad se refiere cada caracter del prompt (0: no relevante, 1: se refiere a Francia, 2: se refiere a capital, 3: se refiere a París).
        var referenciaPromptVerdad = new double[] { 0, 2,2,2,2, 0, 2,2, 0, 2,2, 0, 2,2,2,2,2,2,2, 0, 1,1, 0, 1,1,1,1,1,1,1, 0 };
        var respuesta = "La capital de Francia es París.";
        var referenciaRespuestaPrompt = new double[] { 0,0, 0, 2,2,2,2,2,2,2, 0, 4,4, 0, 4,4,4,4,4,4,4, 0, 4,4, 0, 3,3,3,3,3, 0 };

        //Act & Assert
        var (alucina, detalleFallo) = _helper.EvaluarAlucina(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            umbralPlv,
            factorUmbralMagnitud,
            energia,
            esperado: false);
        alucina.Should().BeFalse(detalleFallo);
    }

    [Theory]
    [InlineData(0.03, 15.0)]
    public void Modelo_ConMuchoRelleno_AlucinaMagnitud(double umbralPlv, double factorUmbralMagnitud)
    {
        //Arrage
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        var energia = prompt.Length;
        // Caracter por caracter se indica a que parte de la verdad se refiere cada caracter del prompt (0: no relevante, 1: se refiere a Francia, 2: se refiere a capital, 3: se refiere a París).
        var referenciaPromptVerdad = new double[] { 0, 2,2,2,2, 0, 2,2, 0, 2,2, 0, 2,2,2,2,2,2,2, 0, 1,1, 0, 1,1,1,1,1,1,1, 0 };
        var respuesta = "Hay muchas repuestas posibles correctas, algunos dicen que es Lyon pero la verdadera capital de Francia es París.";
        var referenciaRespuestaPrompt = new double[] { -5,-5,-5, -5, -5,-5,-5,-5,-5,-5, -5, -5,-5,-5,-5,-5,-5,-5,-5, -5, -5,-5,-5,-5,-5,-5,-5,-5,-5,-5, -5,-5,-5,-5,-5,-5,-5,-5,-5, -5, -5,-5,-5,-5,-5,-5,-5, -5, -5,-5,-5,-5,-5, -5, -5,-5,-5, -5, -5,-5, -5, -5,-5,-5,-5,-5,-5,-5,-5, -5, 2,2,2,2,2,2,2, -5, 4,4, -5, 4,4,4,4,4,4,4, -5, 4,4, -5, 3,3,3,3,3, -5 };

        //Act & Assert
        var (alucina, detalleFallo) = _helper.EvaluarAlucina(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            umbralPlv,
            factorUmbralMagnitud,
            energia,
            esperado: true);
        alucina.Should().BeTrue(detalleFallo);
    }

    [Theory]
    [InlineData(0.03, 15.0)]
    public void Modelo_ConRespuestaFalsa_Alucina(double umbralPlv, double factorUmbralMagnitud)
    {
        //Arrage
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        var energia = prompt.Length;
        // Caracter por caracter se indica a que parte de la verdad se refiere cada caracter del prompt (0: no relevante, 1: se refiere a Francia, 2: se refiere a capital, 3: se refiere a París).
        var referenciaPromptVerdad = new double[] { 0, 2,2,2,2, 0, 2,2, 0, 2,2, 0, 2,2,2,2,2,2,2, 0, 1,1, 0, 1,1,1,1,1,1,1, 0 };
        var respuesta = "Lyon";
        var referenciaRespuestaPrompt = new double[] { -0.1,-0.1,-0.1,-0.1 };

        //Act & Assert
        var (alucina, detalleFallo) = _helper.EvaluarAlucina(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            umbralPlv,
            factorUmbralMagnitud,
            energia,
            esperado: true);
        alucina.Should().BeTrue(detalleFallo);
    }

}
