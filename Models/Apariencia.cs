using System;
using System.Linq;

public class Apariencia
{
    public static Apariencia Vacuidad = new Apariencia(
        t => t == 0 
            ? (double.MaxValue, double.MaxValue) 
            : (0, 0), 
        new Nombre(null, 0, 0));
    public Guid Id { get; }
    public Nombre Esencia { get; }
    public Func<double, (double Amplitud, double Fase)> Valor { get; }

    internal Apariencia(Func<double, (double Amplitud, double Fase)> valor, Nombre esencia)
    {
        Id = Guid.NewGuid();
        Valor = valor;
        Esencia = esencia;
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
                .Select(n => n.Esencia.Valor(t))
                .Aggregate((a, b) => (a.Amplitud + b.Amplitud, a.Fase + b.Fase)),
            designacion.Nombres.First());
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
