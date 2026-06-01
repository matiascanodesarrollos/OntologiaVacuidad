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
        var palabra = nombrePregunta.Mostrarse(respuesta);

        //Assert
        NoAlucina(verdad, respuesta, nombrePregunta, palabra, tolerancia, relevanciaMinima);
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
        var palabra = nombrePregunta.Mostrarse(respuesta);

        //Assert
        Alucina(verdad, respuesta, nombrePregunta, palabra, tolerancia, relevanciaMinima);
    }


    private void NoAlucina(
        string verdad, 
        string respuesta,
        Nombre nombrePregunta, 
        Palabra palabra, 
        double tolerancia,
        double relevanciaMinima)
    {
        for (int tau = 0; tau < respuesta.Length; tau++)
        {
            for (int t = 0; t < verdad.Length; t++)
            {
                var valorPalabra = palabra.Funcion(tau, t);

                var valorAparenciaPura = palabra.Esencia.Apariencia.Funcion(t);
                valorAparenciaPura.Phase.Should().BeApproximately(valorPalabra.Phase, tolerancia, $"en el punto (tau={tau}, t={t})");

                var aparienciaFalsa = palabra.Aparecer(valorPalabra, respuesta);
                var valorAparienciaFalsa = aparienciaFalsa.Apariencia.Funcion(t);
                valorAparienciaFalsa.Phase.Should().BeApproximately(valorPalabra.Phase, tolerancia, $"en el punto (tau={tau}, t={t})");
                valorAparienciaFalsa.Magnitude.Should().BeGreaterThan(valorAparenciaPura.Magnitude, $"en el punto (tau={tau}, t={t})");

                if(valorPalabra.Magnitude > tolerancia)
                {
                    foreach (var omega in nombrePregunta.Fourier.Keys)
                    {
                        var valorDesignacion = aparienciaFalsa.STFT(tau, omega);
                        //Verificar que la respuesta sea relevante para la pregunta en las frecuencias de la pregunta
                        valorDesignacion.Magnitude.Should().BeGreaterThan(relevanciaMinima, $"en el punto (tau={tau}, omega={omega})");
                    }
                }
            }
        }
    }

    private void Alucina(
        string verdad, 
        string respuesta,
        Nombre nombrePregunta, 
        Palabra palabra, 
        double tolerancia,
        double relevanciaMinima)
    {
        var alucina = false;
        for (int tau = 0; tau < respuesta.Length; tau++)
        {
            for (int t = 0; t < verdad.Length; t++)
            {
                var valorPalabra = palabra.Funcion(tau, t);
                var valorAparenciaPura = palabra.Esencia.Apariencia.Funcion(t);
                var aparienciaFalsa = palabra.Aparecer(valorPalabra, respuesta);
                var valorAparienciaFalsa = aparienciaFalsa.Apariencia.Funcion(t);

                if(Math.Abs(valorAparienciaFalsa.Phase - valorAparenciaPura.Phase) > tolerancia)
                {
                    alucina = true;
                    break;
                }

                if(Math.Abs(valorAparienciaFalsa.Magnitude - valorPalabra.Magnitude) > tolerancia)
                {
                    alucina = true;
                    break;
                }

                if(valorPalabra.Magnitude > tolerancia)
                {
                    foreach (var omega in nombrePregunta.Fourier.Keys)
                    {
                        var valorDesignacion = aparienciaFalsa.STFT(tau, omega);
                        //Verificar que la respuesta sea relevante para la pregunta en las frecuencias de la pregunta
                        if (valorDesignacion.Magnitude < relevanciaMinima)
                        {
                            alucina = true;
                            break;
                        }                    
                    }
                }              
                
            }
        }
        alucina.Should().BeTrue("Se esperaba que el modelo alucinara, pero no lo hizo.");
    }
}
