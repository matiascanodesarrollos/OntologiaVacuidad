using System.Numerics;
using ScottPlot;

namespace DomainLogic;

public class AIDiagnostics
{
    public string GenerarDiagnosticos(DetectorAlucinacionIA evaluador)
    {
        var carpetaProyectoTests = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
        var salida = Path.Combine(
            carpetaProyectoTests,
            "TestResults",
            "diagnostics",
            DateTime.UtcNow.ToString("yyyyMMdd_HHmm"));
        Directory.CreateDirectory(salida);
        var carpetaMagnitud = Path.Combine(salida, "magnitud");
        var carpetaFase = Path.Combine(salida, "fase");
        Directory.CreateDirectory(carpetaMagnitud);
        Directory.CreateDirectory(carpetaFase);

        CrearGrafico(evaluador.Prompt.Funcion, $"Prompt", carpetaMagnitud, carpetaFase);
        CrearGrafico(evaluador.Respuesta.Funcion, $"Respuesta", carpetaMagnitud, carpetaFase);

        var metadata = Path.Combine(salida, "metadata.txt");
        File.WriteAllText(
            metadata,
            $"prompt={evaluador.Prompt}{Environment.NewLine}respuesta={evaluador.Respuesta}{Environment.NewLine}");

        return salida;
    }

    private void CrearGrafico(Func<double, Complex> funcion, string tipo, string carpetaMagnitud, string carpetaFase)
    {
        var muestras = 100;
        var periodoMuestreo = 0.01;
        var magnitud = new double[muestras];
        var fase = new double[muestras];
        for (var n = 0; n < muestras; n++)
        {
            var t = (n - muestras / 2) * periodoMuestreo;
            var valor = funcion(t);

            magnitud[n] = valor.Magnitude > 0 ? valor.Magnitude : 0.0;
            fase[n] = valor.Phase > 0 ? valor.Phase : 0.0;
        }
        GuardarSerie(magnitud, $"{tipo} Magnitud", "t", "|A(t)|", Path.Combine(carpetaMagnitud, $"{tipo.ToLower()}_magnitud.png"));
        GuardarSerie(fase, $"{tipo} Fase", "t", "fase(rad)", Path.Combine(carpetaFase, $"{tipo.ToLower()}_fase.png"));
    }

    private void GuardarSerie(double[] serie, string titulo, string ejeX, string ejeY, string ruta)
    {
        var centerIndex = serie.Length / 2;
        var xs = Enumerable
            .Range(-centerIndex, serie.Length)
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