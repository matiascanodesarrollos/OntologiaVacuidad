using System;
using System.Collections.Generic;
using System.Linq;
public class Apariencia
{
    public Guid Id { get; }
    public Designacion Esencia { get; }
    public IList<Nombre> Naturalezas { get; internal set; }
    public double Amplitud { get; internal set; } = 1;
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

    internal Apariencia(Designacion esencia, Nombre causa)
    {
        Id = Guid.NewGuid();
        Esencia = esencia;
        Naturalezas = new List<Nombre> { causa };
    }

    internal double Modular(Nombre nombreProyectado)
    {
        var frecuenciaMaxima = Naturalezas.Max(x => Math.Abs(x.Causa.Frecuencia));
        if(Math.Abs(nombreProyectado.Causa.Frecuencia) <= frecuenciaMaxima)
        {
            if(!Naturalezas.Any(c => c.Id == nombreProyectado.Id))
            {
                Naturalezas.Add(nombreProyectado);
            }
            //Modulación AM, simula la multiplicación de la función de onda portadora por el mensaje
            Amplitud *= 1 + nombreProyectado.Efecto.Amplitud;
        } 
        return Amplitud;
    }
    
    public override string ToString()
    {
        return $"Apariencia: {string.Join(", ", EnteDesignado)}";
    }
}
