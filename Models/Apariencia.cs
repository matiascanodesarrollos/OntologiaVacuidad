using System;
using System.Collections.Generic;
using System.Linq;

public class Apariencia : Designacion
{
    public Func<double, (double EjeReal, double EjeImaginario)> Funcion { get; }

    internal Apariencia(Designacion designacion)
        : base(designacion.Nombres.ToList())
    {
        Funcion = t => Nombres.Count() > 1 
            ? Nombres
                .Select(nombre =>
                {
                    var frecuencia = nombre.Frecuencia;
                    var fase = nombre.Fase;
                    var amplitud = nombre.Amplitud;
                    return (amplitud * Math.Cos(frecuencia * t + fase),
                            amplitud * Math.Sin(frecuencia * t + fase));
                })
                .Aggregate((acumulado, actual) => 
                    (acumulado.Item1 + actual.Item1, 
                    acumulado.Item2 + actual.Item2))
            : t == 0 ? (double.PositiveInfinity, double.PositiveInfinity) : (0.0, 0.0);
    }

    /// <summary>
    /// Crea una nueva apariencia a partir de una designación, sumando las amplitudes de las apariencias de los nombres que componen la designación (Fourrier).
    /// </summary>
    /// <param name="nombres">Los nombres a partir de los cuales se crea la apariencia.</param>
    /// <returns>Una nueva apariencia creada a partir de la designación.</returns>
    public static Apariencia Aparecer(List<Nombre> nombres)
    {
        var designacion = new Designacion(nombres);
        var apariencia = new Apariencia(designacion);
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

    /// <summary>
    /// Apariencia base. Delta de Dirac.
    /// </summary>
    public static Apariencia Mente = new Apariencia(Vacuidad);
}

