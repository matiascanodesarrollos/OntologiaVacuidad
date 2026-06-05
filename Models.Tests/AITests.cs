using FluentAssertions;
using System.Numerics;
using ScottPlot;

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
        var alucina = Alucina(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            toleranciaDefase, 
            factorUmbralMagnitud,
            energia,
            forzarDiagnostico: true,
            out var detalleFallo);
        alucina.Should().BeFalse(detalleFallo);
    }

    [Theory]
    [InlineData(Math.PI / 2, 5.0)]
    [InlineData(Math.PI / 2, 10.0)]
    [InlineData(Math.PI / 2, 20.0)]
    [InlineData(Math.PI / 2, 30.0)]
    public void Modelo_ConMuchoRelleno_Alucina(double toleranciaDefase, double factorUmbralMagnitud)
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
        var alucina = Alucina(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            toleranciaDefase, 
            factorUmbralMagnitud,
            energia,
            forzarDiagnostico: true,
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
        double factorUmbralMagnitud,
        double energia,
        bool forzarDiagnostico,
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

        var aparienciaPregunta = new Apariencia(
            nombrePregunta.Fourier.Sum(p => p.Key), 
            t =>
                {
                    if (t == 0)
                    {
                        return new Complex(energia, 0.0);
                    }

                    if (t > prompt.Length || t < 0)
                    {
                        return Complex.Zero;
                    }

                    return new Complex(energia * Math.Exp(-t), energia * t);
                }, 
            energia);
        var palabra = nombrePregunta.Mostrarse(aparienciaPregunta);

        var designacion = new Designacion(
            aparienciaPregunta,
            nombreRespuesta);

        var alucina = false;
        detalleFallo = $"Sin incumplimiento de umbrales. Fase={toleranciaDefase:F6}, Magnitud={energia:F6}";
        foreach (var omega in designacion.Fourier.Keys)
        {
            for (int tau = 0; tau < prompt.Length; tau++)
            {
                for (int t = 0; t < respuesta.Length; t++)
                {
                    var valorPalabra = palabra.Funcion(tau, t);
                    var valorApariencia = aparienciaPregunta.Funcion(t);
                    var valorDesignacion = designacion.STFT(tau, omega);                      
                    var deltaMagnitud = Math.Abs(valorApariencia.Magnitude - valorPalabra.Magnitude);
                    var umbralMagnitud = energia * factorUmbralMagnitud;
                    if (deltaMagnitud > umbralMagnitud){
                        alucina = true;
                        detalleFallo = $"Magnitud: delta={deltaMagnitud:F6} > umbral={umbralMagnitud:F6}, apariencia={valorApariencia.Magnitude:F2}, palabra={valorPalabra.Magnitude:F2}, t={t}, tau={tau}, omega={omega:F6}";
                        break;
                    }

                    var umbralMagnitudFase = Math.Max(1e-6, energia * 0.01);
                    if (!double.IsFinite(valorApariencia.Magnitude)
                        || !double.IsFinite(valorPalabra.Magnitude)
                        || valorApariencia.Magnitude <= umbralMagnitudFase
                        || valorPalabra.Magnitude <= umbralMagnitudFase)
                    {
                        continue;
                    }

                    var faseApariencia = Math.Abs(valorApariencia.Phase);
                    var faseDesignacion = Math.Abs(valorDesignacion.Phase);
                    var deltaFase = faseApariencia - faseDesignacion;
                    if (deltaFase > toleranciaDefase)
                    {
                        alucina = true;
                        detalleFallo = $"Fase: delta={deltaFase:F6} > umbral={toleranciaDefase:F6}, apariencia={faseApariencia:F2}, designacion={faseDesignacion:F2}, t={t}, omega={omega:F6}";
                        break;
                    }                
                }
            }            

            if (alucina)
            {
                break;
            }                        
        }

        if (forzarDiagnostico || DebeGenerarDiagnostico() || alucina)
        {
            var carpetaDiagnostico = GenerarDiagnosticos(
                palabra,
                designacion,
                aparienciaPregunta,
                prompt.Length,
                respuesta.Length,
                toleranciaDefase,
                energia);
            detalleFallo = $"{detalleFallo} | Diagnostico={carpetaDiagnostico}";
        }
        

        return alucina;
    }

    private static bool DebeGenerarDiagnostico()
    {
        var variable = Environment.GetEnvironmentVariable("AI_DIAG");
        return string.Equals(variable, "1", StringComparison.OrdinalIgnoreCase)
            || string.Equals(variable, "true", StringComparison.OrdinalIgnoreCase);
    }

    private string GenerarDiagnosticos(
        Palabra palabra,
        Designacion designacion,
        Apariencia apariencia,
        int totalPrompt,
        int totalRespuesta,
        double toleranciaDefase,
        double energia)
    {
        var carpetaProyectoTests = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
        var salida = Path.Combine(
            carpetaProyectoTests,
            "TestResults",
            "diagnostics",
            $"diag_{DateTime.UtcNow:yyyyMMdd_HHmmss_fff}_{Guid.NewGuid():N}");
        Directory.CreateDirectory(salida);
        var carpetaMagnitud = Path.Combine(salida, "magnitud");
        var carpetaFase = Path.Combine(salida, "fase");
        Directory.CreateDirectory(carpetaMagnitud);
        Directory.CreateDirectory(carpetaFase);

        var omegas = designacion.Fourier.Keys.OrderBy(omega => omega).ToArray();

        var palabraMagnitud = new double[totalRespuesta, totalPrompt];
        var palabraFase = new double[totalRespuesta, totalPrompt];
        var stftMagnitud = new double[totalRespuesta, omegas.Length];
        var stftFase = new double[totalRespuesta, omegas.Length];
        var aparienciaMagnitud = new double[totalPrompt];
        var aparienciaFase = new double[totalPrompt];

        for (int tau = 0; tau < totalRespuesta; tau++)
        {
            for (int t = 0; t < totalPrompt; t++)
            {
                var valorPalabra = palabra.Funcion(tau, t);
                palabraMagnitud[tau, t] = valorPalabra.Magnitude;
                palabraFase[tau, t] = valorPalabra.Phase;
            }

            for (int i = 0; i < omegas.Length; i++)
            {
                var valorStft = designacion.STFT(tau, omegas[i]);
                stftMagnitud[tau, i] = valorStft.Magnitude;
                stftFase[tau, i] = valorStft.Phase;
            }
        }

        for (int t = 0; t < totalPrompt; t++)
        {
            var valorApariencia = apariencia.Funcion(t);
            aparienciaMagnitud[t] = valorApariencia.Magnitude;
            aparienciaFase[t] = valorApariencia.Phase;
        }

        GuardarHeatmap(palabraMagnitud, "Palabra Magnitud", "t", "tau", Path.Combine(carpetaMagnitud, "palabra_magnitud.png"), "Escala magnitud-color");
        GuardarHeatmap(stftMagnitud, "STFT Magnitud", "omega-index", "tau", Path.Combine(carpetaMagnitud, "stft_magnitud.png"), "Escala magnitud-color");
        GuardarSerie(aparienciaMagnitud, "Apariencia Magnitud", "t", "|A(t)|", Path.Combine(carpetaMagnitud, "apariencia_magnitud.png"));

        GuardarHeatmap(palabraFase, "Palabra Fase", "t", "tau", Path.Combine(carpetaFase, "palabra_fase.png"));
        GuardarHeatmap(stftFase, "STFT Fase", "omega-index", "tau", Path.Combine(carpetaFase, "stft_fase.png"));
        GuardarSerie(aparienciaFase, "Apariencia Fase", "t", "fase(rad)", Path.Combine(carpetaFase, "apariencia_fase.png"));

        var metadata = Path.Combine(salida, "metadata.txt");
        File.WriteAllText(
            metadata,
            $"prompt={totalPrompt}{Environment.NewLine}respuesta={totalRespuesta}{Environment.NewLine}omegas={omegas.Length}{Environment.NewLine}toleranciaDefase={toleranciaDefase:F6}{Environment.NewLine}energia={energia:F6}{Environment.NewLine}");

        return salida;
    }

    private void GuardarHeatmap(double[,] datos, string titulo, string ejeX, string ejeY, string ruta, string etiquetaEscala = "Valor")
    {
        var plot = new Plot();
        var heatmap = plot.Add.Heatmap(datos);
        var colorBar = plot.Add.ColorBar(heatmap);
        colorBar.Label = etiquetaEscala;
        plot.Title(titulo);
        plot.XLabel(ejeX);
        plot.YLabel(ejeY);
        plot.SavePng(ruta, 1200, 800);
    }

    private void GuardarSerie(double[] serie, string titulo, string ejeX, string ejeY, string ruta)
    {
        var xs = Enumerable.Range(0, serie.Length).Select(i => (double)i).ToArray();
        var plot = new Plot();
        plot.Add.Scatter(xs, serie);
        plot.Title(titulo);
        plot.XLabel(ejeX);
        plot.YLabel(ejeY);
        plot.SavePng(ruta, 1200, 800);
    }
}
