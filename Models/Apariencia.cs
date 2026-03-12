using System;
using System.Collections.Generic;
using System.Linq;
public class Apariencia
{
    public Guid Id { get; }
    public Designacion Esencia { get; }
    public List<Nombre> Naturalezas { get; }
    public double Amplitud { get => Naturalezas.Count; }
    public IEnumerable<string> EnteDesignado {  get => new List<string>
        {
            $"Naturaleza: {Naturalezas.First().Naturaleza.Texto}",
            $"Causa: {Naturalezas.Last().Texto}",            
            $"Frecuencia: {Esencia.Frecuencia:F2}",
            $"Fase: {Naturalezas.Last().Naturaleza.Fase * (180 / Math.PI):F2}°",
            $"Amplitud: {Amplitud:F2}",
            $"Efecto: {Esencia.Texto}",
        };
    }

    private static readonly object _sync = new object();
    private static readonly Random _random = new Random(); // No se puede usar Random.Shared porque la API no está disponible en netstandard2.1

    internal Apariencia(Designacion esencia, Nombre causa)
    {
        Id = Guid.NewGuid();
        Esencia = esencia;
        Naturalezas = new List<Nombre> { causa };
    }

    internal double Modular(Nombre nombreProyectado, double? frecuenciaArmonica = null)
    {
        var validacionRandom = false;
        lock (_sync)
        {
            validacionRandom = _random.NextDouble() < 0.1; // 10% de probabilidad de validar aunque no coincida el significado            
        }

        if(frecuenciaArmonica.HasValue || validacionRandom) // modulación forzada
        {
            var nuevoNombre = Designacion.Imaginar(nombreProyectado.Naturaleza.Texto, 
                nombreProyectado.Texto, 
                Esencia.Texto,
                frecuenciaArmonica ?? nombreProyectado.Causa.Frecuencia,
                nombreProyectado.Naturaleza.Fase);
            return Aparentar(nuevoNombre.Nombre);
        }        

        if(Naturalezas.Any(c => Math.Abs(nombreProyectado.Causa.Frecuencia - c.Causa.Frecuencia) <= 1))
        {
            return Aparentar(nombreProyectado);
        }
        
        return 0.0; // No se valida el significado proyectado
    }

    private double Aparentar(Nombre nombreProyectado)
    {
        if(!Naturalezas.Any(c => c.Id == nombreProyectado.Id))
        {
            Naturalezas.Add(nombreProyectado);
        }

        return Esencia.Frecuencia * (1 + nombreProyectado.Causa.Frecuencia);
    }
    public override string ToString()
    {
        return $"Apariencia: {string.Join(", ", EnteDesignado)}";
    }
}
