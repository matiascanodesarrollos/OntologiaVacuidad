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
        CrearGrafico(contextoBuilder.NombrePromt.Esencia, "Prompt", carpetaMagnitud, carpetaFase, tMax);
        CrearGrafico(contextoBuilder.NombreRespuesta.Esencia, "Respuesta", carpetaMagnitud, carpetaFase, tMax);

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
}