using ScottPlot;

namespace Models.Tests;

internal static class AITestDiagnostics
{
    internal static bool DebeGenerarDiagnostico()
    {
        var variable = Environment.GetEnvironmentVariable("AI_DIAG");
        return string.Equals(variable, "1", StringComparison.OrdinalIgnoreCase)
            || string.Equals(variable, "true", StringComparison.OrdinalIgnoreCase);
    }

    internal static string GenerarDiagnosticos(
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

    private static void GuardarHeatmap(double[,] datos, string titulo, string ejeX, string ejeY, string ruta, string etiquetaEscala = "Valor")
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

    private static void GuardarSerie(double[] serie, string titulo, string ejeX, string ejeY, string ruta)
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