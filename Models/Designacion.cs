using System;
using System.Numerics;

public class Designacion : Nombre
{
    public Guid Id { get; }
    public Palabra Causa { get; }
    public Apariencia Esencia { get; }
    public Func<(double tau, double FrecuenciaAngular), Complex> STFT { get; }
    
    /// <summary>
    /// Constructor de copia para crear una nueva designación a partir de otra para herencia.
    /// <param name="otra">La designación de la cual se copiarán las propiedades.</param>
    /// </summary>   
    public Designacion(Designacion otra)
        : base(otra.Texto, otra.Contexto, otra.Ventana)
    {
        Id = Guid.NewGuid();
        Causa = otra.Causa;
        Esencia = otra.Esencia;
        STFT = otra.STFT;
    }

    /// <summary>
    /// Crea una designación calculando una STFT con la funcion de la apariencia y la ventana del nombre.
    /// La funcion en si de la STFT se puede sobreescribir para implementar diferentes formas de análisis.
    /// Si la apariencia es una palabra, es su causa de la designación. 
    /// Su esencia es la apariencia de entrada.
    /// </summary>
    /// <param name="apariencia">Apariencia de entrada.</param>
    /// <param name="nombre">Nombre que aporta la ventana de análisis.</param>
    /// <returns>Una nueva designación vinculada a la apariencia de entrada.</returns>
    public Designacion(Apariencia apariencia, Nombre nombre)
        : base(nombre.Texto, nombre.Contexto, nombre.Ventana)
    {
        Id = Guid.NewGuid();
        Causa = apariencia as Palabra;
        Esencia = apariencia;
        STFT = p => CalcularSTFT(p.tau, p.FrecuenciaAngular);
    }

    public static Designacion Vacuidad(double energia) => new Designacion(
        Apariencia.Mente(energia),
        new Nombre(
            nameof(Vacuidad),
            nameof(Apariencia.Mente),
            t => new Complex(0.0, t / (2 * Math.PI))
        ) //Transformada inversa de δ′
    );

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
    /// Calcula la STFT discreta completa usando un paso temporal de 1 por caracter del contexto.
    /// </summary>
    /// <param name="tau">Desplazamiento temporal de la ventana.</param>
    /// <param name="frecuenciaAngular">Frecuencia angular de analisis.</param>
    /// <returns>Valor complejo de la STFT en el punto (tau, frecuenciaAngular).</returns>
    protected virtual Complex CalcularSTFT(double tau, double frecuenciaAngular)
    {
        var totalMuestras = Math.Max(1, Contexto.Length);

        var suma = Complex.Zero;

        for (var n = 0; n < totalMuestras; n++)
        {
            var t = n + 1.0;
            var x = Esencia.Funcion(t);
            var w = Complex.Conjugate(Ventana(t - tau));
            var exponente = Complex.FromPolarCoordinates(1.0, -frecuenciaAngular * t);

            suma += x * w * exponente;
        }

        return suma;
    }
}
