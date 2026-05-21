using System;
using System.Numerics;

public class Palabra
{
    public Guid Id { get; }
    public double Frecuencia { get; }
    public Func<double, Complex> Fase { get; }
    public Func<double, Complex> Ventana { get; }
    public string Texto { get; }

    internal Palabra(
        string texto, 
        double frecuencia, 
        Func<double, Complex> ventana)
    {
        Id = Guid.NewGuid();
        Frecuencia = frecuencia;
        Texto = texto;
        Fase = t => Complex.FromPolarCoordinates(1.0, 2 * Math.PI * frecuencia * t);
        Ventana = ventana;
    }

    // Palabra base con ventana gaussiana centrada en cero.
    public static Palabra Yo(double frecuencia) => 
        new Palabra(
            nameof(Yo), 
            frecuencia, 
            t => Math.Exp(-(t * t) / 2.0) //Gaussiana
    );
}
