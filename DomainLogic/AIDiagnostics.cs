using ScottPlot;

namespace DomainLogic;

public class AIDiagnostics
{
    public string GenerarDiagnosticos(ContextBuilder contextoBuilder)
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
        var carpetaHeatMap = Path.Combine(salida, "heatmap");
        Directory.CreateDirectory(carpetaMagnitud);
        Directory.CreateDirectory(carpetaFase);
        Directory.CreateDirectory(carpetaHeatMap);
        
        var tMax = Math.Max(contextoBuilder.Prompt.Length, contextoBuilder.Respuesta.Length);
        CrearGrafico(contextoBuilder.AparienciaPromt, "Prompt", carpetaMagnitud, carpetaFase, tMax);
        CrearGrafico(contextoBuilder.AparienciaRespuesta, "Respuesta", carpetaMagnitud, carpetaFase, tMax);
        CrearHeatMap(contextoBuilder, carpetaHeatMap, tMax);

        var metadata = Path.Combine(salida, "metadata.txt");
        File.WriteAllText(
            metadata,
            $"prompt={contextoBuilder.Prompt}{Environment.NewLine}respuesta={contextoBuilder.Respuesta}{Environment.NewLine}");

        return salida;
    }

    private void CrearGrafico(Apariencia apariencia, string tipo, string carpetaMagnitud, string carpetaFase, int tMax)
    {
        var muestrasPorUnidad = 10;
        var muestras = tMax * muestrasPorUnidad; // 10 muestras por unidad de tiempo
        var magnitud = new double[muestras];
        var fase = new double[muestras];
        for (var t = 0.01; t <= tMax; t += 1.0 / muestrasPorUnidad)
        {
            var valor = apariencia.Funcion(t);

            var indice = (int)((t - 1.0 / muestrasPorUnidad) * muestrasPorUnidad);
            magnitud[indice] = valor.Magnitude;
            fase[indice] = valor.Phase;
        }
        GuardarSerie(magnitud, $"{tipo} Magnitud", "t", "|A(t)|", Path.Combine(carpetaMagnitud, $"{tipo.ToLower()}_magnitud.png"));
        GuardarSerie(fase, $"{tipo} Fase", "t", "fase(rad)", Path.Combine(carpetaFase, $"{tipo.ToLower()}_fase.png"));
    }

    private void GuardarSerie(double[] serie, string titulo, string ejeX, string ejeY, string ruta)
    {
        var xs = Enumerable
            .Range(0, serie.Length)
            .Select(i => (double)i)
            .ToArray();
        var plot = new Plot();
        plot.Add.Scatter(xs, serie);
        plot.Title(titulo);
        plot.XLabel(ejeX);
        plot.YLabel(ejeY);
        plot.SavePng(ruta, 1200, 800);
    }

    private void CrearHeatMap(ContextBuilder contextoBuilder, string carpetaHeatMap, int tMax)
    {
        var designacion = contextoBuilder.CrearDesignacion();
        var muestrasPorUnidad = 10;
        var muestrasTau = Math.Max(tMax * muestrasPorUnidad, 1);
        var omegasPrompt = contextoBuilder.NombrePromt.Fourier.Keys.ToHashSet();
        var omegasRespuesta = contextoBuilder.NombreRespuesta.Fourier.Keys.ToHashSet();
        var omegas = omegasPrompt
            .Union(omegasRespuesta)
            .OrderBy(omega => omega)
            .ToArray();

        if (omegas.Length == 0)
        {
            return;
        }

        var magnitudesStft = new double[omegas.Length, muestrasTau];
        for (var iOmega = 0; iOmega < omegas.Length; iOmega++)
        {
            for (var iTau = 0; iTau < muestrasTau; iTau++)
            {
                var tau = iTau / (double)muestrasPorUnidad;
                magnitudesStft[iOmega, iTau] = designacion.STFT(tau, omegas[iOmega]).Magnitude;
            }
        }

        var plot = new Plot();
        plot.Add.Heatmap(magnitudesStft);
        for (var iOmega = 0; iOmega < omegas.Length; iOmega++)
        {
            var enPrompt = omegasPrompt.Contains(omegas[iOmega]);
            var enRespuesta = omegasRespuesta.Contains(omegas[iOmega]);
            var linea = plot.Add.HorizontalLine(iOmega);
            linea.Color = ObtenerColorEtiqueta(enPrompt, enRespuesta);
            linea.LineWidth = 1;
        }
        plot.Title("Designacion STFT | Magnitud");
        plot.XLabel("tau");
        plot.YLabel("omega (rojo=prompt, verde=ambos, transparente=respuesta)");
        plot.SavePng(Path.Combine(carpetaHeatMap, "designacion_stft_heatmap.png"), 1200, 800);
    }

    private static Color ObtenerColorEtiqueta(bool enPrompt, bool enRespuesta)
    {
        if (enPrompt && enRespuesta)
        {
            return Colors.Green;
        }

        if (enPrompt)
        {
            return Colors.Red;
        }

        return Colors.Transparent;
    }
}