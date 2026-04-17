using System;
using System.Linq;

public class Apariencia
{
    public virtual Guid Id { get; }
    public Func<double, (double, double)> Amplitud { get; internal set; }

    internal Apariencia(Func<double, (double, double)> amplitud)
    {
        Id = Guid.NewGuid();
        Amplitud = amplitud;
    }

    /// <summary>
    /// Crea una nueva apariencia a partir de una designación, sumando las amplitudes de las apariencias de los nombres que componen la designación (Fourrier).
    /// </summary>
    /// <param name="designacion">La designación a partir de la cual se crea la apariencia.</param>
    /// <returns>Una nueva apariencia creada a partir de la designación.</returns>
    public static Apariencia Aparecer(Designacion designacion)
    {
        var apariencia = new Apariencia(t => //Fourrier
            designacion
                .Nombres
                .Select(n => n.Esencia.Amplitud(t))
                .Aggregate((a, b) => (a.Item1 + b.Item1, a.Item2 + b.Item2)));
        return apariencia;
    }


    /// <summary>
    /// Sobreescribe GetHashCode para comparar apariencias por su Id.
    /// </summary>
    /// <returns>El hash code de la apariencia.</returns>
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    /// Sobreescribe Equals para comparar apariencias por su Id.
    /// </summary>
    /// <returns>True si las apariencias son iguales, false en caso contrario.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Apariencia other)
        {
            return Id == other.Id;
        }
        return false;
    }
}
