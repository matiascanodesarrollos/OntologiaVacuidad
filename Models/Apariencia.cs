using System;
using System.Numerics;

public class Apariencia
{
    public Guid Id { get; }    
    public Func<double, Complex> Funcion { get; }
    public Designacion Esencia { get; }
    public Lazy<double> Amplitud { get; } 

    /// <summary>
    /// Crea una copia pública de otra apariencia preservando su comportamiento para herencia.
    /// /// <param name="otra">La apariencia de la cual se copiarán las propiedades.</param>
    /// </summary>
    public Apariencia(Apariencia otra)
    {
        Id = otra.Id;        
        Funcion = otra.Funcion;
        Esencia = otra.Esencia;
        Amplitud = otra.Amplitud;
    }

    internal Apariencia(
        string texto,
        string contexto, 
        double frecuenciaAngular, 
        Func<double, Complex> ventana)
    {
        Id = Guid.NewGuid();
        var nombre = new Nombre(
                texto, 
                contexto, 
                ventana);
        Amplitud = new Lazy<double>(() => 
            nombre.Fourier.ContainsKey(frecuenciaAngular) 
                ? nombre.Fourier[frecuenciaAngular].Magnitude 
                : 1.0);
        Funcion = t => Amplitud.Value * Complex.FromPolarCoordinates(1.0, frecuenciaAngular * t);
        Esencia = new Designacion(
            this, 
            nombre);        
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

    public static Apariencia Mente(double energia) => new Apariencia(//Transformada inversa de u(ω)
        nameof(Mente),
        nameof(Designacion.Vacuidad),
        0.0,
        t => new Complex(
            t == 0.0 ? 0.5 * energia : 0.0, 
            t == 0.0 ? energia : 1 / (2 * Math.PI * t))
    );
}

