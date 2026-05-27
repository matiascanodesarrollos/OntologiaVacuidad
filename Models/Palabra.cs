using System;
using System.Numerics;

public class Palabra : Apariencia
{
    public new Func<double, double, Complex> Funcion { get; }

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
    }

    public Complex Aparecer(Complex z, double omega)
    {
        var muestras = Math.Max(1, Contexto.Length);
        var suma = Complex.Zero;
        for (int n = 0; n < muestras; n++)
        {
            var designacion = Esencia.STFT(n, omega);
            suma += Funcion(n, n) * Complex.Pow(z, -n);
        }
        return suma;
    }
}
