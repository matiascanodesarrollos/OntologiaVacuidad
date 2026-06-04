using FluentAssertions;
using System.Numerics;

namespace Models.Tests;

public class AITests
{
    [Theory]
    [InlineData(2.5, 6000)]
    public void Modelo_ConRespuestaCorrecta_NoAlucina(double toleranciaDefase, double energiaMaxima)
    {
        //Arrage
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        // Caracter por caracter se indica a que parte de la verdad se refiere cada caracter del prompt (0: no relevante, 1: se refiere a Francia, 2: se refiere a capital, 3: se refiere a París).
        var referenciaPromptVerdad = new double[] { 0, 2, 2, 2, 2, 0, 2, 2, 0, 2, 2, 0, 2, 2, 2, 2, 2, 2, 2, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0 };
        var respuesta = "La capital de Francia es París.";
        // Caracter por caracter se indica a que parte del prompt se refiere cada caracter de la respuesta (0: no relevante, 1: se refiere al contexto de la pregunta, 2: se refiere a la pregunta en sí, 3: se refiere a la respuesta).
        var referenciaRespuestaPrompt = new double[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

        //Act & Assert
        var alucina = Alucina(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            toleranciaDefase, 
            energiaMaxima,
            out var detalleFallo);
        alucina.Should().BeFalse(detalleFallo);
    }

    [Theory]
    [InlineData(2.5, 5500)]
    public void Modelo_ConMuchoRelleno_Alucina(double toleranciaDefase, double energiaMaxima)
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
            toleranciaDefase, 
            energiaMaxima,
            out var detalleFallo);
        alucina.Should().BeTrue(detalleFallo);
    }


    private bool Alucina(
        string verdad, 
        string prompt,
        string respuesta,
        double[] referenciaPromptVerdad,
        double[] referenciaRespuestaPrompt,
        double toleranciaDefase,
        double energiaMaxima,
        out string detalleFallo)
    {
        Func<double, Complex> funcionTemporalPregunta = t =>
            {
                var indice = (int)t;
                if (indice >= 0 && indice < referenciaPromptVerdad.Length)
                {
                    return referenciaPromptVerdad[indice];
                }
                return 0;
            };
        var nombrePregunta = new Nombre(
                    prompt,
                    verdad,
                    funcionTemporalPregunta,
                    1.0);
        Func<double, Complex> funcionTemporalRespuesta = t =>
            {
                var indice = (int)t;
                if (indice >= 0 && indice < referenciaRespuestaPrompt.Length)
                {
                    return referenciaRespuestaPrompt[indice];
                }
                return 0;
            };
        var nombreRespuesta = new Nombre(
                    respuesta,
                    prompt,
                    funcionTemporalRespuesta,
                    1.0);

        var energiaPregunta = CalcularEnergia(nombrePregunta);
        var aparienciaPregunta = new Apariencia(
            nombrePregunta.Fourier.Sum(p => p.Key), 
            funcionTemporalPregunta, 
            energiaPregunta);
        var palabra = nombrePregunta.Mostrarse(aparienciaPregunta);

        var nombreVerdad = new Nombre(
                    verdad,
                    verdad,
                    t => t <= 0
                        ? new Complex(0.5 * energiaPregunta, 0.0)
                        : new Complex(0.0, energiaPregunta / (2 * Math.PI * t)),
                    1.0);
        var designacion = new Designacion(
            aparienciaPregunta,
            nombreVerdad);

        var alucina = false;
        detalleFallo = "Sin incumplimiento de umbrales.";
        for (int tau = 0; tau < respuesta.Length; tau++)
        {
            for (int t = 0; t < prompt.Length; t++)
            {
                var valor = palabra.Funcion(tau, t);
                foreach (var omega in designacion.Fourier.Keys)
                {
                    if(t == tau)
                    {
                        var stftValor = designacion.STFT(tau, omega);
                        var decaimientoVerdad = nombrePregunta.Ventana(t).Magnitude * Math.Exp(-Math.Abs(t));
                        var magnitudStftAjustada = stftValor.Magnitude * decaimientoVerdad;
                        var deltaMagnitud = Math.Abs(magnitudStftAjustada - valor.Magnitude);
                        if (deltaMagnitud > energiaMaxima)
                        {
                            alucina = true;
                            detalleFallo = $"Magnitud: delta={deltaMagnitud:F6} > umbral={energiaMaxima:F6}, designacionAjustada={magnitudStftAjustada:F2}, designacionCruda={stftValor.Magnitude:F2}, palabra={valor.Magnitude:F2}, decaimientoVerdad={decaimientoVerdad:F6}, tau={tau}, t={t}, omega={omega:F6}.";
                            break;
                        }

                        var faseStftNormalizada = NormalizarFase(stftValor.Phase);
                        var faseValorNormalizada = NormalizarFase(valor.Phase);
                        var deltaFase = Math.Abs(faseStftNormalizada - faseValorNormalizada);
                        if (deltaFase > toleranciaDefase && faseValorNormalizada > toleranciaDefase)
                        {
                            alucina = true;
                            detalleFallo = $"Fase: delta={deltaFase:F6} > umbral={toleranciaDefase:F6}, designacion={faseStftNormalizada:F2}, palabra={faseValorNormalizada:F2}, tau={tau}, t={t}, omega={omega:F6}.";
                            break;
                        }
                    }                
                }

                if (alucina)
                {
                    break;
                }
            }

            if (alucina)
            {
                break;
            }
        }
        return alucina;
    }

    private double CalcularEnergia(Nombre nombre)
    {        
        return nombre.Fourier.Sum(p => p.Value.Magnitude);
    }

    private static double NormalizarFase(double fase)
    {
        var dosPi = 2.0 * Math.PI;
        var normalizada = fase % dosPi;
        if (normalizada <= -Math.PI)
        {
            normalizada += dosPi;
        }
        else if (normalizada > Math.PI)
        {
            normalizada -= dosPi;
        }
        return Math.Abs(normalizada);
    }
}
