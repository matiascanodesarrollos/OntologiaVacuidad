using System;
using System.Linq;
using System.Numerics;

public class Designacion : Nombre
{
    public new Guid Id { get; }
    public Palabra Esencia { get; }
    public Func<double, double, Complex> STFT { get; }

    /// <summary>
    /// Crea una designación con una STFT predefinida.
    /// </summary>
    /// <param name="nombre">Nombre asociado a la designación.</param>
    /// <param name="texto">Texto de la palabra.</param>
    /// <param name="tiempoPalabra">Momento relativo a tau en que se pronuncia la palabra.</param>
    public Designacion(Nombre nombre, string texto, double tiempoPalabra)
        : base(nombre)
    {
        Id = Guid.NewGuid();
        Esencia = new Palabra(texto, this, nombre.Fourier.Sum(p => p.Key));
        STFT = (tau, omega) => nombre.Fourier.TryGetValue(omega, out var valor) 
            ? valor * Esencia.Funcion(tau, tiempoPalabra) 
            : Complex.Zero;
    }
    
    /// <summary>
    /// Crea una designación calculando una STFT con la funcion de la apariencia y la ventana del nombre.
    /// La funcion en si de la STFT se puede sobreescribir para implementar diferentes formas de análisis.
    /// Su esencia es la apariencia de entrada.
    /// </summary>
    /// <param name="apariencia">Apariencia de entrada.</param>
    /// <param name="nombre">Nombre que aporta la ventana de análisis.</param>
    /// <returns>Una nueva designación vinculada a la apariencia de entrada.</returns>
    public Designacion(Apariencia apariencia, Nombre nombre)
        : base(nombre)
    {
        Id = Guid.NewGuid();
        Esencia = apariencia.Causa;
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
            var t = n; // Paso temporal de 1 por caracter del contexto            
            var x = apariencia.Funcion(t);
            var w = Complex.Conjugate(Ventana(t - tau));
            var exponente = Complex.FromPolarCoordinates(1.0, -omega * t);

            suma += x * w * exponente;
        }

        return suma;
    }
}
