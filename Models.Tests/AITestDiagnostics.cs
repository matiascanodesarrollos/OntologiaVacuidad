using ScottPlot;

namespace Models.Tests;

public class AITestDiagnostics
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
        Directory.CreateDirectory(carpetaMagnitud);
        Directory.CreateDirectory(carpetaFase);
        
        var tMax = Math.Max(contextoBuilder.Prompt.Length, contextoBuilder.Respuesta.Length);
        CrearGrafico(contextoBuilder.AparienciaPromt, "Prompt", carpetaMagnitud, carpetaFase, tMax);
        CrearGrafico(contextoBuilder.AparienciaRespuesta, "Respuesta", carpetaMagnitud, carpetaFase, tMax);

        var metadata = Path.Combine(salida, "metadata.txt");
        File.WriteAllText(
            metadata,
            $"prompt={contextoBuilder.Prompt}{Environment.NewLine}respuesta={contextoBuilder.Respuesta}{Environment.NewLine}");

        return salida;
    }

    private static void CrearGrafico(Apariencia apariencia, string tipo, string carpetaMagnitud, string carpetaFase, int tMax)
    {
        var magnitud = new double[tMax];
        var fase = new double[tMax];
        for (int t = 0; t < tMax; t++)
        {
            var valorInicial = apariencia.Funcion(t);
            var valorFinal = apariencia.Funcion(t + 1.0);
            var valor = 0.5 * (valorInicial + valorFinal);

            magnitud[t] = valor.Magnitude;
            fase[t] = valor.Phase;
        }
        GuardarSerie(magnitud, $"{tipo} Magnitud", "t", "|A(t)|", Path.Combine(carpetaMagnitud, $"{tipo.ToLower()}_magnitud.png"));
        GuardarSerie(fase, $"{tipo} Fase", "t", "fase(rad)", Path.Combine(carpetaFase, $"{tipo.ToLower()}_fase.png"));
    }

    private static void GuardarSerie(double[] serie, string titulo, string ejeX, string ejeY, string ruta)
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