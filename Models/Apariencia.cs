using System;
using System.Numerics;

public class Apariencia
{
    public Guid Id { get; }    
    public Func<double, Complex> Funcion { get; }
    public Lazy<Complex> Amplitud { get; }
    public double FrecuenciaAngular { get; }
    public Nombre Esencia { get; set; }

    internal Apariencia(double frecuenciaAngular)
    {
        Id = Guid.NewGuid();
        FrecuenciaAngular = frecuenciaAngular;
        Amplitud = new Lazy<Complex>(() => 
            Esencia.CalcularFourier(FrecuenciaAngular)
        );
        Funcion = t => 
            Amplitud.Value * Complex.FromPolarCoordinates(1, FrecuenciaAngular * t);
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

