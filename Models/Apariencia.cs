using System;
using System.Collections.Generic;
using System.Linq;

public class Apariencia
{
    public Guid Id { get; }
    protected List<Nombre> _nombres { get; set; }
    public IEnumerable<Nombre> Nombres => _nombres.AsReadOnly();
    public Func<double, (double EjeReal, double EjeImaginario)> Valor { get; }

    internal Apariencia(Func<double, (double EjeReal, double EjeImaginario)> valor, List<Nombre> esencia)
    {
        Id = Guid.NewGuid();
        Valor = valor;
        _nombres = esencia;
    }

    /// <summary>
    /// Crea una nueva apariencia a partir de una designación, sumando las amplitudes de las apariencias de los nombres que componen la designación (Fourrier).
    /// </summary>
    /// <param name="designacion">La designación a partir de la cual se crea la apariencia.</param>
    /// <returns>Una nueva apariencia creada a partir de la designación.</returns>
    public static Apariencia Aparecer(Designacion designacion)
    {
        var nombres = designacion.Nombres.ToList();
        var apariencia = new Apariencia(t => //Fourrier
            nombres
                .Select(n => n.Esencia.Valor(t))
                .Aggregate((a, b) => (a.EjeReal + b.EjeReal, a.EjeImaginario + b.EjeImaginario)),
            nombres);
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
    /// Apariencia base.
    /// </summary>
    public static Apariencia Vacuidad = new Apariencia(
        t => t == 0 
            ? (double.MaxValue, double.MaxValue) 
            : (0, 0), 
        new List<Nombre>());
}

