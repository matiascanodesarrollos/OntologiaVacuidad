using FluentAssertions;

namespace Models.Tests;

public class AITests
{
    [Theory]
    [InlineData(0.01, 50)]
    [InlineData(0.01, 500)]
    [InlineData(0.01, 1000)]
    [InlineData(0.01, 5000)]
    public void Modelo_ConRespuestaCorrectaYRelevante_NoAlucina(double tolerancia, double relevanciaMinima)
    {
        //Arrage
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        // Caracter por caracter se indica a que parte de la verdad se refiere cada caracter del prompt (0: no relevante, 1: se refiere a Francia, 2: se refiere a capital, 3: se refiere a París).
        var referenciaPromptVerdad = new double[] { 0, 2, 2, 2, 2, 0, 2, 2, 0, 2, 2, 0, 2, 2, 2, 2, 2, 2, 2, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0 };
        var respuesta = "La capital de Francia es París.";
        // Caracter por caracter se indica a que parte del prompt se refiere cada caracter de la respuesta (0: no relevante, 1: se refiere al contexto de la pregunta, 2: se refiere a la pregunta en sí, 3: se refiere a la respuesta).
        var referenciaRespuestaPrompt = new double[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

        //Act
        var nombrePregunta = new Nombre(
                    prompt,
                    verdad,
                    t =>
                    {
                        var indice = (int)t;
                        if (indice >= 0 && indice < referenciaPromptVerdad.Length)
                        {
                            return referenciaPromptVerdad[indice];
                        }
                        return 0;
                    },
                    1.0);
        var nombreRespuesta = new Nombre(
                    respuesta,
                    prompt,
                    t =>
                    {
                        var indice = (int)t;
                        if (indice >= 0 && indice < referenciaRespuestaPrompt.Length)
                        {
                            return referenciaRespuestaPrompt[indice];
                        }
                        return 0;
                    },
                    1.0);
        var designacion = new Designacion(
            nombreRespuesta,
            0.0);
        var palabra = nombrePregunta.Mostrarse(designacion, respuesta);

        //Assert
        var alucina = !Alucina(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            tolerancia, 
            relevanciaMinima);
        alucina.Should().BeFalse("Se esperaba que el modelo no alucinara, pero lo hizo.");
    }

    [Theory]
    [InlineData(0.01, 50)]
    [InlineData(0.01, 25)]
    [InlineData(0.01, 10)]
    [InlineData(0.01, 1)]
    public void Modelo_ConRespuestaCorrectaYMuchoRelleno_Alucina(double tolerancia, double relevanciaMinima)
    {
        //Arrage
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        // Caracter por caracter se indica a que parte de la verdad se refiere cada caracter del prompt (0: no relevante, 1: se refiere a Francia, 2: se refiere a capital, 3: se refiere a París).
        var referenciaPromptVerdad = new double[] { 0, 2, 2, 2, 2, 0, 2, 2, 0, 2, 2, 0, 2, 2, 2, 2, 2, 2, 2, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0 };
        var respuesta = "Hay muchas repuestas posibles correctas, algunos dicen que es Lyon pero la verdadera capital de Francia es París.";
        // Caracter por caracter se indica a que parte del prompt se refiere cada caracter de la respuesta (0: no relevante, 1: se refiere al contexto de la pregunta, 2: se refiere a la pregunta en sí, 3: se refiere a la respuesta).
        var referenciaRespuestaPrompt = new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

        //Act & Assert
        var alucina = Alucina(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            tolerancia, 
            relevanciaMinima);
        alucina.Should().BeTrue("Se esperaba que el modelo alucinara, pero no lo hizo.");
    }


    private bool Alucina(
        string verdad, 
        string prompt,
        string respuesta,
        double[] referenciaPromptVerdad,
        double[] referenciaRespuestaPrompt,
        double tolerancia,
        double relevanciaMinima)
    {
        var nombrePregunta = new Nombre(
                    prompt,
                    verdad,
                    t =>
                    {
                        var indice = (int)t;
                        if (indice >= 0 && indice < referenciaPromptVerdad.Length)
                        {
                            return referenciaPromptVerdad[indice];
                        }
                        return 0;
                    },
                    1.0);
        var nombreRespuesta = new Nombre(
                    respuesta,
                    prompt,
                    t =>
                    {
                        var indice = (int)t;
                        if (indice >= 0 && indice < referenciaRespuestaPrompt.Length)
                        {
                            return referenciaRespuestaPrompt[indice];
                        }
                        return 0;
                    },
                    1.0);
        var designacion = new Designacion(
            nombreRespuesta,
            0.0);
        var palabra = nombrePregunta.Mostrarse(designacion, respuesta);

        var alucina = true;
        for (int tau = 0; tau < respuesta.Length; tau++)
        {
            for (int t = 0; t < verdad.Length; t++)
            {
                
            }
        }
        return alucina;
    }
}
