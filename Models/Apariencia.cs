using System;
using System.Numerics;

public class Apariencia
{
    public Guid Id { get; }
    public Func<double, Complex> Funcion { get; }
    internal Designacion Esencia { get; set; }

    internal Apariencia(double frecuenciaAngular)
    {
        Id = Guid.NewGuid();
        var amplitud = new Lazy<Complex>(() => Esencia.Fourier(frecuenciaAngular));
        Funcion = t => amplitud.Value * Complex.FromPolarCoordinates(1, frecuenciaAngular * t);
    }

    internal Apariencia(Func<double, Complex> funcion)
    {
        Id = Guid.NewGuid();
        Funcion = funcion;
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

