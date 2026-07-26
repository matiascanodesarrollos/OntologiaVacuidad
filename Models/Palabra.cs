using System;
using System.Numerics;

public class Palabra
{
    public string Texto { get; }
    public Func<double, Complex> Admitancia { get; } 

    /// <summary>
    /// Crea una palabra y su apariencia correspondiente.
    /// La función de la palabra modela la respiración:
    /// Devuelve la presión en la parte real y el flujo de aire en la imaginaria.
    /// </summary>
    /// <param name="texto">Texto que se dijo.</param>
    /// <param name="admitancia">Función de admitancia para esa frecuencia.</param>
    internal Palabra(
        string texto,
        Func<double, Complex> admitancia)
    {
        Texto = texto;
        Admitancia = admitancia;
    }
}
