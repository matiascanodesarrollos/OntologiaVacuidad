using System;
using System.Collections.Generic;
using System.Linq;
public class Apariencia
{
    public Guid Id { get; }
    public Designacion Naturaleza { get; }
    public IList<Nombre> Efectos { get; internal set; }
    public int Amplitud { get; internal set; } = 1;
    public IEnumerable<string> EnteDesignado {  get => new List<string>
        {
            $"Naturaleza: {Efectos.First().Naturaleza.Texto}",
            $"Causa: {Efectos.Last().Texto}",            
            $"Frecuencia: {Naturaleza.Frecuencia:F2}",
            $"Fase: {Efectos.Last().Naturaleza.Fase * (180 / Math.PI):F2}°",
            $"Amplitud: {Amplitud}",
            $"Efecto: {Naturaleza.Texto}",
        };
    }

    internal Apariencia(Designacion esencia, Nombre causa)
    {
        Id = Guid.NewGuid();
        Naturaleza = esencia;
        Efectos = new List<Nombre> { causa };
    }

    internal int Modular(Nombre nombreProyectado)
    {
        var frecuenciaMaxima = Efectos.Max(x => Math.Abs(x.Causa.Frecuencia));
        if(Math.Abs(nombreProyectado.Causa.Frecuencia) <= frecuenciaMaxima)
        {
            if(!Efectos.Any(c => c.Id == nombreProyectado.Id))
            {
                Efectos.Add(nombreProyectado);
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
