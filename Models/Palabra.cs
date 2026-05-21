using System;
using System.Numerics;

public class Palabra
{
    public Guid Id { get; }
    public double FrecuenciaAngular { get; }
    public Func<double, Complex> Fase { get; }
    public Func<double, Complex> Ventana { get; }
    public string Texto { get; }

    internal Palabra(
        string texto, 
        double frecuenciaAngular,
        Func<double, Complex> ventana)
    {
        Id = Guid.NewGuid();
        FrecuenciaAngular = frecuenciaAngular;
        Texto = texto;
        Fase = t => Complex.FromPolarCoordinates(1.0, frecuenciaAngular * t);
        Ventana = ventana;
    }

    // Palabra base con ventana gaussiana centrada en cero.
    public static Palabra Yo(double frecuenciaAngular) =>
        new Palabra(
            nameof(Yo), 
            frecuenciaAngular,
            t => Math.Exp(-(t * t) / 2.0) //Gaussiana
    );
}
