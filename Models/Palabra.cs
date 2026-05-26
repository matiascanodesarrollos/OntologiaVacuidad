using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class Palabra : Apariencia
{
    public string Texto { get; }
    public string Contexto { get; }
    public double FrecuenciaAngular { get; }
    public Func<double, Complex> Fase { get; }
    public Func<double, Complex> Ventana { get; }

    internal Palabra(
        string texto,
        string contexto,
        double frecuenciaAngular,
        Func<double, Complex> ventana)
        : base(texto, contexto, frecuenciaAngular, ventana)
    {
        Texto = texto;
        Contexto = contexto;
        FrecuenciaAngular = frecuenciaAngular;        
        Fase = tau => Complex.FromPolarCoordinates(1.0, frecuenciaAngular * tau);
        Ventana = ventana;
    }

    /// <summary>
    /// Crea una apariencia compuesta a partir de una lista de palabras.
    /// </summary>
    /// <param name="palabras">Palabras de entrada que se combinan en una sola señal.</param>
    /// <returns>Una apariencia cuya ventana es la productoria de fase por ventana de cada palabra.</returns>
    public static Palabra Aparecer(IEnumerable<Palabra> palabras)
    {
        var lista = palabras.ToList();
        var palabra = new Palabra(
            string.Join(" ", lista.Select(p => p.Texto)),
            string.Join(" ", lista.Select(p => p.Contexto)),
            lista.Sum(p => p.FrecuenciaAngular),
            t => lista.Aggregate(
                Complex.One,
                (prod, p) => prod * p.Ventana(t)
            )
        );
        return palabra;
    }
}
