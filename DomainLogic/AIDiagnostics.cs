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

        CrearGrafico((t) => evaluador.Prompt.Funcion(t, 0), $"Prompt", carpetaMagnitud, carpetaFase);
        var aparienciaPrompt = evaluador.Prompt as Apariencia;
        CrearGrafico(aparienciaPrompt.Funcion, $"Prompt_Apariencia", carpetaMagnitud, carpetaFase);
        CrearGrafico(evaluador.Prompt.Efecto.Ventana, $"Prompt_Ventana", carpetaMagnitud, carpetaFase);

        CrearGrafico((t) => evaluador.Respuesta.Funcion(t, 0), $"Respuesta", carpetaMagnitud, carpetaFase);
        var aparienciaRespuesta = evaluador.Respuesta as Apariencia;
        CrearGrafico(aparienciaRespuesta.Funcion, $"Respuesta_Apariencia", carpetaMagnitud, carpetaFase);
        CrearGrafico(evaluador.Respuesta.Efecto.Ventana, $"Respuesta_Ventana", carpetaMagnitud, carpetaFase);

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
            var t = n * periodoMuestreo;
            var valor = funcion(t);

            magnitud[n] = valor.Magnitude > 0 ? valor.Magnitude : 0.0;
            fase[n] = valor.Phase > 0 ? valor.Phase : 0.0;
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