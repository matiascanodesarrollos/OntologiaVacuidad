using System;
using System.Collections.Generic;
using System.Linq;
public class Apariencia
{
    public Guid Id { get; }
    public Designacion Esencia { get; }
    public List<Nombre> Significados { get; }
    public double Amplitud { get => Significados.Count; }
    public IEnumerable<string> EnteDesignado {  get => new List<string>
        {
            $"Naturaleza: {Significados.First().Naturaleza.Texto}",
            $"Causa: {Significados.Last().Texto}",            
            $"Frecuencia: {Esencia.Frecuencia:F2}",
            $"Fase: {Significados.Last().Naturaleza.Fase * (180 / Math.PI):F2}°",
            $"Amplitud: {Amplitud:F2}",
            $"Efecto: {Esencia.Texto}",
        };
    }

    private static readonly Random _random = new Random();

    internal Apariencia(Designacion esencia, Nombre causa)
    {
        Id = Guid.NewGuid();
        Esencia = esencia;
        Significados = new List<Nombre> { causa };
    }

    /// <summary>
    /// Simula modulación AM s(f)=p(f)*(1+m(f)))
    /// </summary>
    /// <param name="nombreProyectado">El nombre proyectado sobre la apariencia.</param>
    /// <param name="frecuenciaArmonica">Si no es nulo, fuerza la modulación.</param>
    /// <returns>La frecuencia modulada.</returns>
    internal double Modular(Nombre nombreProyectado, double? frecuenciaArmonica = null)
    {
        if(frecuenciaArmonica.HasValue || _random.NextDouble() < 0.1) // modulación forzada o 10% de probabilidad de validar aunque no coincida el significado 
        {
            var nuevoNombre = Designacion.Imaginar(nombreProyectado.Naturaleza.Texto, 
                nombreProyectado.Texto, 
                Esencia.Texto,
                frecuenciaArmonica ?? nombreProyectado.Causa.Frecuencia,
                nombreProyectado.Naturaleza.Fase);
            return Aparentar(nuevoNombre.Nombre);
        }

        if(Significados.Any(c => Math.Abs(nombreProyectado.Causa.Frecuencia - c.Causa.Frecuencia) <= 1))
        {
            return Aparentar(nombreProyectado);
        }
        
        return 0.0; // No se valida el significado proyectado
    }

    private double Aparentar(Nombre nombreProyectado)
    {
        if(!Significados.Any(c => c.Id == nombreProyectado.Id))
        {
            Significados.Add(nombreProyectado);
        }

        return Esencia.Frecuencia * (1 + nombreProyectado.Causa.Frecuencia);
    }
    public override string ToString()
    {
        return $"Apariencia: {string.Join(", ", EnteDesignado)}";
    }
}
