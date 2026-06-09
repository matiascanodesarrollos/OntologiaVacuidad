using System;
using System.Linq;
using System.Numerics;

public class Apariencia
{
    public Guid Id { get; }
    public double FrecuenciaAngular { get; }
    public Func<double, Complex> Funcion { get; }
    public Lazy<Palabra> Causa { get; internal set; }
    public Lazy<Complex> Amplitud { get; }

    public Apariencia(Nombre nombre)
    {
        Id = Guid.NewGuid();
        FrecuenciaAngular = nombre.Fourier.Sum(p => p.Key);
        Amplitud = new Lazy<Complex>(() => CalcularAmplitud(nombre, FrecuenciaAngular));
        Funcion = t => 
            Amplitud.Value * Complex.FromPolarCoordinates(1, FrecuenciaAngular * t);
        Causa = new Lazy<Palabra>(() => Palabra.Gozo(Amplitud.Value.Magnitude));
    }

    internal Apariencia(double frecuenciaAngular, Func<double, Complex> funcion, double energia)
    {
        Id = Guid.NewGuid();
        FrecuenciaAngular = frecuenciaAngular;
        Funcion = funcion;
        Causa = new Lazy<Palabra>(() => Palabra.Gozo(energia));
    }

    /// <summary>
    /// Calcula la amplitud como la integral discreta de la ventana sobre el contexto.
    /// Sobreescribir para definir otro criterio de amplitud.
    /// </summary>
    /// <param name="nombre">Nombre que aporta la ventana de análisis.</param>
    /// <param name="omega">Frecuencia angular de análisis.</param>
    /// <returns>El integral complejo de la ventana.</returns>
    public virtual Complex CalcularAmplitud(Nombre nombre, double omega)
    {
        var muestras = Math.Max(1, nombre.Contexto.Length);
        var integral = Complex.Zero;

        // Integral discreta con paso temporal unitario por caracter del contexto.
        for (var t = 0; t < muestras; t++)
        {
            var muestra = nombre.Ventana(t);
            var factor = Complex.FromPolarCoordinates(1.0, -omega * t);
            integral += muestra * factor;
        }

        return integral;
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

