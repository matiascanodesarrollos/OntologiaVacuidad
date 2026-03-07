using System;
using System.Collections.Generic;
public class Apariencia
{
    public Guid Id { get; }
    public Designacion Esencia { get; }
    public Nombre Causa { get; }
    public double Amplitud { get; internal set; }
    public IEnumerable<string> EnteDesignado {  get => new List<string>
        {
            $"Naturaleza: {Causa.Naturaleza.Texto}",
            $"Causa: {Causa.Texto}",            
            $"Frecuencia: {Esencia.Frecuencia:F2}",
            $"Fase: {Causa.Naturaleza.Fase * (180 / Math.PI):F2}°",
            $"Amplitud: {Amplitud:F2}",
            $"Efecto: {Esencia.Verbo}",
        };
    }

    internal Apariencia(Designacion esencia, Nombre causa)
    {
        Id = Guid.NewGuid();
        Esencia = esencia;
        Causa = causa;
        Amplitud = 1.0;
    }
    public override string ToString()
    {
        return $"Apariencia: {string.Join(", ", EnteDesignado)}.";
    }
}
