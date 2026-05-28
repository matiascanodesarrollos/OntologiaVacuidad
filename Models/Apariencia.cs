using System;
using System.Numerics;

public class Apariencia : Nombre
{
    public Func<double, Complex> Funcion { get; }
    public Palabra Causa { get; protected set; }
    public Designacion Esencia { get; }
    public double Amplitud { get; } 

    internal Apariencia(
        Nombre nombre,
        double omega)
        : base(nombre)
    {
        Amplitud = Fourier[omega].Magnitude;
        Funcion = t => Complex.FromPolarCoordinates(Amplitud, omega * t);
        Esencia = new Designacion(
            this, 
            this);
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

