using System;
using System.Collections.Generic;

public class Nombre
{
    public Guid Id { get; }
    public Palabra Naturaleza { get; }
    public Designacion Causa { get; internal set; }
    public Apariencia Efecto { get; internal set;}
    public string Texto { get; }

    internal Nombre(string sustantivo, Palabra naturaleza, Apariencia efecto)
    {
        Id = Guid.NewGuid();
        Naturaleza = naturaleza;
        Efecto = efecto;
        Texto = sustantivo;
    }

    public List<Apariencia> BuscarSignificado()
    {
        var resultado = new List<Apariencia>();
        Nombre actual = this;
        while (actual.Efecto.Esencia.Frecuencia != 0)
        {
            actual.Efecto.Amplitud++;
            resultado.Add(actual.Efecto);
            actual = actual.Causa.Nombre;
        }
        return resultado;
    }

    public override string ToString()
    {
        return $"Nombre: {Texto}, Naturaleza: {Naturaleza.Texto}, Fase: {Naturaleza.Fase:F2}";
    }
}
