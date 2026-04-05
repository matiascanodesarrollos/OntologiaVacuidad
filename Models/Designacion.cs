using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Designacion : Apariencia
{
    public override Guid Id { get; }
    public (double X, double Y) Velocidad
    {
        get
        {
            if(Nombres.Count == 1)
            {
                return (Math.Cos(Nombres[0].Fase), Math.Sin(Nombres[0].Fase));
            }

            var nombresOrdenados = Nombres.OrderByDescending(n => n.Efecto.Amplitud).ToList();
            return (Math.Cos(nombresOrdenados[0].Fase), Math.Sin(nombresOrdenados[1].Fase));
        }
    }

    public List<Nombre> Nombres { get; internal set; }

    internal Designacion(List<Nombre> nombres)
        : base(nombres.First())
    {
        Id = Guid.NewGuid();
        Nombres = nombres;
    }

    /// <summary>
    /// Sobreescribe ToString para mostrar una representación de la designación, incluyendo su naturaleza y esencia.
    /// Se muestra la naturaleza como una lista de efectos, cada uno con su causa, frecuencia y fase.
    /// </summary>
    /// <returns>Una cadena que representa la designación.</returns>
    public override string ToString()
    {
        var resultado = new StringBuilder();
        resultado.AppendLine("═══ Designación ═══");
        foreach (var efecto in Nombres)
        {
            if(efecto.Frecuencia == 0)
            {
                break;
            }
            resultado.AppendLine(efecto.ToString());
        }
        resultado.AppendLine("═══ Fin ═══");
        return resultado.ToString();
    }

    /// <summary>
    /// Sobreescribe Equals para comparar designaciones por su Id.
    /// </summary>
    /// <returns>True si las designaciones son iguales, false en caso contrario.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Designacion other)
        {
            return Id == other.Id;
        }
        return false;
    }

    /// <summary>
    /// Sobreescribe GetHashCode para comparar designaciones por su Id.
    /// </summary>
    /// <returns>El hash code de la designación.</returns>
    public override int GetHashCode() => Id.GetHashCode();
}
