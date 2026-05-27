using System;
using System.Numerics;

public class Palabra : Apariencia
{
    public new Func<double, double, Complex> Funcion { get; }

    internal Palabra(
        string texto,
        string contexto,
        double frecuenciaAngular,
        Func<double, Complex> ventana)
        : base(texto, contexto, frecuenciaAngular, ventana)
    {      
        Funcion = (tau, t) => 
            Complex.FromPolarCoordinates(1.0, frecuenciaAngular * tau) 
            * ventana(t - tau);
        Causa = this;
    }
}
