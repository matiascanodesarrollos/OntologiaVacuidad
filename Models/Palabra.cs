using System;
using System.Numerics;

public class Palabra
{
    public string Texto { get; }
    public Func<double, Complex> Admitancia { get; } 

    /// <summary>
    /// Crea una palabra con texto y admitancia.   
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
