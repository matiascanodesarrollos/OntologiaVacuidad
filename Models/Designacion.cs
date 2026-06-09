using System;
using System.Numerics;

public class Designacion : Nombre
{
    public new Guid Id { get; }
    public Palabra Esencia { get; }
    public Func<double, double, Complex> STFT { get; }
    
    internal Designacion(Apariencia apariencia, Nombre nombre)
        : base(nombre)
    {
        Id = Guid.NewGuid();
        Esencia = apariencia.Causa.Value;
        STFT = (tau, omega) => CalcularSTFT(tau, omega);
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

    /// <summary>
    /// Calcula la STFT evaluando la función de la esencia en la ventana del nombre 
    /// con un paso temporal de 1 por caracter del contexto.
    /// Sobreescribir para implementar diferentes formas de análisis o pasos temporales.
    /// </summary>
    /// <param name="tau">Desplazamiento temporal de la ventana.</param>
    /// <param name="omega">Frecuencia angular de analisis.</param>
    /// <returns>Valor complejo de la STFT en el punto (tau, omega).</returns>
    protected virtual Complex CalcularSTFT(double tau, double omega)
    {        
        var totalMuestras = Math.Max(1, Contexto.Length);
        var suma = Complex.Zero;
        var apariencia = Esencia as Apariencia;

        for (var n = 0; n < totalMuestras; n++)
        {
            var muestra = apariencia.Funcion(n);
            var ventana = Complex.Conjugate(Ventana(n - tau));
            var factor = Complex.FromPolarCoordinates(1.0, -omega * n);
            suma += muestra * ventana * factor; // Paso temporal de 1 por caracter del contexto  
        }

        return suma;
    }
}
