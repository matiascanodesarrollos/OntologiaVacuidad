using System;
using System.Collections.Generic;
public class Apariencia
{
    public Guid Id { get; }
    public Designacion Esencia { get; }
    public Nombre Causa { get; }
    public double Amplitud { get; set; }
    public IEnumerable<string> EnteDesignado {  get => new List<string>
        {
            $"Naturaleza: {Causa.Naturaleza.Texto}",
            $"Causa: {Causa.Texto}",
            $"Efecto: {Esencia.Verbo}",
            $"Frecuencia: {Esencia.Frecuencia:F2}",
            $"Fase: {Causa.Naturaleza.Fase:F2}",
            $"Amplitud: {Amplitud:F2}",
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
