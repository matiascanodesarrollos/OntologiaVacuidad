using System;
using System.Numerics;

public class Palabra
{
    public string Texto { get; }
    public double FrecuenciaAngular { get; }
    public Func<double, Complex> Admitancia { get; } 

    protected Palabra(Palabra otra)
    {
        Texto = otra.Texto;
        FrecuenciaAngular = otra.FrecuenciaAngular;
        Admitancia = otra.Admitancia;
    }

    /// <summary>
    /// Crea una palabra con texto, frecuencia respiratoria y admitancia.
    /// </summary>
    /// <param name="texto">Texto que se dijo.</param>
    /// <param name="frecuenciaAngular">Frecuencia angular de la respiración.</param>
    /// <param name="admitancia">Función de admitancia.</param>
    public Palabra(
        string texto,
        double frecuenciaAngular,
        Func<double, Complex> admitancia)
    {
        Texto = texto;
        FrecuenciaAngular = frecuenciaAngular;
        Admitancia = admitancia;
    }
}
