using System.Numerics;
using FluentAssertions;

namespace Models.Tests;

public class AITests
{
    [Fact]
    public void Modelo_ConRespuestaRelevante_NoAlucina()
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
        var (nombrePregunta, palabra, designacion) = EjecutarCaso(verdad, prompt, referenciaPromptVerdad, respuesta, referenciaRespuestaPrompt);

        //Assert
        NoAlucina(verdad, respuesta, nombrePregunta, palabra, designacion, 0.5);
    }

    private (Nombre NombrePregunta, Palabra Palabra, Designacion Designacion) 
        EjecutarCaso(string verdad, string prompt, double[] referenciaPromptVerdad, string respuesta, double[] referenciaRespuestaPrompt)
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
        var palabra = nombrePregunta.Mostrarse(respuesta);
        var designacion = new Designacion(palabra, nombreRespuesta);
        return (nombrePregunta, palabra, designacion);
    }

    private void NoAlucina(
        string verdad, 
        string respuesta,
        Nombre nombrePregunta, 
        Palabra palabra, 
        Designacion designacion,
        double tolerancia)
    {
        for (int tau = 0; tau < respuesta.Length; tau++)
        {
            for (int t = 0; t < verdad.Length; t++)
            {
                var valorPalabra = palabra.Funcion(tau, t);
                if(valorPalabra.Magnitude > tolerancia)
                {
                    foreach (var omega in nombrePregunta.Fourier.Keys)
                    {
                        var valorDesignacion = designacion.STFT(tau, omega);
                        //Verificar que la respuesta sea relevante para la pregunta en las frecuencias de la pregunta
                        valorDesignacion.Magnitude.Should().BeGreaterThan(tolerancia, $"en el punto (tau={tau}, omega={omega})");
                    }
                }                
                var apariencia = palabra.Aparecer(valorPalabra, respuesta);
                var valorApariencia = apariencia.Apariencia.Funcion(t);
                valorApariencia.Phase.Should().BeApproximately(valorPalabra.Phase, tolerancia, $"en el punto (tau={tau}, t={t})");
            }
        }
    }

    private void Alucina(
        string verdad, 
        string respuesta,
        Nombre nombrePregunta, 
        Palabra palabra, 
        Designacion designacion,
        double tolerancia)
    {
        var alucina = false;
        for (int tau = 0; tau < respuesta.Length; tau++)
        {
            for (int t = 0; t < verdad.Length; t++)
            {
                var valorPalabra = palabra.Funcion(tau, t);
                if(valorPalabra.Magnitude > tolerancia)
                {
                    foreach (var omega in nombrePregunta.Fourier.Keys)
                    {
                        var valorDesignacion = designacion.STFT(tau, omega);
                        //Verificar que la respuesta sea relevante para la pregunta en las frecuencias de la pregunta
                        if (valorDesignacion.Magnitude < tolerancia)
                        {
                            alucina = true;
                            break;
                        }                    
                    }
                }                
                var apariencia = palabra.Aparecer(valorPalabra, respuesta);
                var valorApariencia = apariencia.Apariencia.Funcion(t);
                if(Math.Abs(valorApariencia.Magnitude - valorPalabra.Magnitude) > tolerancia)
                {
                    alucina = true;
                    break;
                }
            }
        }
        alucina.Should().BeTrue("Se esperaba que el modelo alucinara, pero no lo hizo.");
    }
}
