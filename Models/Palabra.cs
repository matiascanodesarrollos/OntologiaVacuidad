using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class Palabra : Apariencia
{
    public new Func<double, double, Complex> Funcion { get; }
    public double Omega { get; }

    internal Palabra(
        string texto,
        string contexto,
        double omega,
        Func<double, Complex> ventana)
        : base(texto, contexto, omega, ventana)
    {      
        Funcion = (tau, t) => 
            Complex.FromPolarCoordinates(1.0, omega * tau) 
            * ventana(t - tau);
        Causa = this;
        Omega = omega;
    }

    /// <summary>
    /// Crea una palabra a partir de un conjunto, combinando sus textos, contextos, omegas y ventanas.
    /// La función de la palabra resultante es el producto de las funciones de las palabras individuales.
    /// </summary>
    /// <param name="palabras"></param>
    public Palabra(IEnumerable<Palabra> palabras)
        : base(
            string.Join(".", palabras.Select(p => p.Texto)),
            string.Join(".", palabras.Select(p => p.Contexto)),
            palabras.Sum(p => p.Omega),
            t => palabras.Aggregate(Complex.One, (acc, p) => acc * p.Ventana(t))
        )
    {
        Causa = palabras.FirstOrDefault();
    }

    /// <summary>
    /// Crea una nueva apariencia a partir de esta palabra en un punto z del plano complejo.
    /// Usa el argumento de z como frecuencia angular de analisis (omega = arg(z))
    /// y construye la salida acumulando terminos z^{-n}.
    /// </summary>
    /// <param name="z">Variable compleja de evaluacion en la transformada Z.</param>
    /// <returns>Una nueva apariencia de ventana constante igual a la transformada Z de su esencia.</returns>
    public Apariencia Aparecer(Complex z)
    {
        var muestras = Math.Max(1, Contexto.Length);
        var omega = z.Phase;
        var suma = Complex.Zero;
        for (int n = 0; n < muestras; n++)
        {
            var designacion = Esencia.STFT(n, omega);
            suma += Funcion(n, n) * Complex.Pow(z, -n);
        }
        return new Apariencia(
            Texto,
            Contexto,
            omega,
            t => suma * Math.Pow(z.Magnitude, -t)
        );
    }
}
