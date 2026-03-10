using System;
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

    /// <summary>
    /// Modulacion FM
    /// </summary>
    /// <param name="frecuencia">La frecuencia a modular.</param>
    /// <returns>La frecuencia modulada.</returns>
    internal double Modular(double[] frecuencias)
    {
        foreach (var frecuencia in frecuencias)
        {
            if(Math.Abs(Causa.Frecuencia - frecuencia) <= 1)
            {
                return frecuencia; // Modulación FM
            }
        }

        return 0.0;
    }

    public override string ToString()
    {
        return $"Nombre: {Texto}, Naturaleza: {Naturaleza.Texto}, Fase: {Naturaleza.Fase * (180 / Math.PI):F2}º, Frecuencia: {Causa.Frecuencia:F2} Hz.";
    }
}
