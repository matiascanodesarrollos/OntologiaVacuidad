using System;
using System.Numerics;

public class Designacion
{
    public Guid Id { get; }
    public Apariencia Esencia { get; }
    public Nombre NombreProyectado { get; }
    public Func<(double tau, double FrecuenciaAngular), Complex> STFT { get; }
    
    /// <summary>
    /// Crea una designación calculando una STFT con la funcion de la apariencia y la ventana del nombre.
    /// La funcion en si de la STFT se puede sobreescribir para implementar diferentes formas de análisis.
    /// Su esencia es la apariencia de entrada.
    /// </summary>
    /// <param name="apariencia">Apariencia de entrada.</param>
    /// <param name="nombre">Nombre que aporta la ventana de análisis.</param>
    /// <returns>Una nueva designación vinculada a la apariencia de entrada.</returns>
    public Designacion(Apariencia apariencia, Nombre nombre)
    {
        Id = Guid.NewGuid();
        Esencia = apariencia;
        NombreProyectado = nombre;
        STFT = p => CalcularSTFT(p.tau, p.FrecuenciaAngular);
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
    /// Calcula la STFT discreta completa usando un paso temporal de 1 por caracter del contexto.
    /// </summary>
    /// <param name="tau">Desplazamiento temporal de la ventana.</param>
    /// <param name="frecuenciaAngular">Frecuencia angular de analisis.</param>
    /// <returns>Valor complejo de la STFT en el punto (tau, frecuenciaAngular).</returns>
    protected virtual Complex CalcularSTFT(double tau, double frecuenciaAngular)
    {
        var totalMuestras = Math.Max(1, Esencia.Contexto.Length);

        var suma = Complex.Zero;

        for (var t = 0; t < totalMuestras; t++)
        {
            var x = Esencia.Funcion(t);
            var w = Complex.Conjugate(NombreProyectado.Ventana(t - tau));
            var exponente = Complex.FromPolarCoordinates(1.0, -frecuenciaAngular * t);

            suma += x * w * exponente;
        }

        return suma;
    }

    public static Designacion Vacuidad(double energia) //Transformada inversa de δ′(ω)
        => new Designacion(
            Apariencia.Mente(energia),
            new Nombre(
                nameof(Vacuidad),
                nameof(Apariencia.Mente),
                t => new Complex(0.0, t / (2 * Math.PI))
        ) 
    );
}
