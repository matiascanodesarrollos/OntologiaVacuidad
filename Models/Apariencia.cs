using System;
using System.Collections.Generic;
using System.Linq;
public class Apariencia
{
    public Guid Id { get; }
    public Designacion Esencia { get; }
    public IEnumerable<Nombre> Naturalezas => _naturalezas;
    private List<Nombre> _naturalezas;
    public double Amplitud { get => Naturalezas.Count(); }
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

    private static readonly Random _random = new Random(); // La probabilidad que busco obtener no es crítica por lo que no importa manejar multiples hilos

    internal Apariencia(Designacion esencia, Nombre causa)
    {
        Id = Guid.NewGuid();
        Esencia = esencia;
        _naturalezas = new List<Nombre> { causa };
    }

    internal double Modular(Nombre nombreProyectado, double? frecuenciaForzada = null)
    {
        if(frecuenciaForzada.HasValue || _random.NextDouble() < 0.1) // 10% de probabilidad de validar aunque no coincida el significado
        {
            var nuevoDesignacion = Designacion.Crear(Esencia.Nombre.Texto, 
                Esencia.Texto,
                Esencia.Nombre.Naturaleza.Texto,
                frecuenciaForzada ?? nombreProyectado.Causa.Frecuencia,
                nombreProyectado.Naturaleza.Fase);
            return Aparentar(nuevoDesignacion.Nombre);
        }

        if(Naturalezas.Any(c => Math.Abs(nombreProyectado.Causa.Frecuencia - c.Causa.Frecuencia) <= 1))
        {
            return Aparentar(nombreProyectado);
        }
        
        return 0.0; // No se valida el significado proyectado
    }

    private double Aparentar(Nombre nombreProyectado)
    {
        if(!_naturalezas.Any(c => c.Id == nombreProyectado.Id))
        {
            _naturalezas.Add(nombreProyectado);
        }

        //Modulación AM, simula la multiplicación de la función de onda portadora por el mensaje
        return Esencia.Frecuencia * (1 + nombreProyectado.Causa.Frecuencia);
    }
    public override string ToString()
    {
        return $"Apariencia: {string.Join(", ", EnteDesignado)}";
    }
}
