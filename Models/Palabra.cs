using System;
using System.Numerics;

public class Palabra : Apariencia
{
    public string Texto { get; }
    public double FrecuenciaAngular { get; }
    public Func<double, Complex> Fase { get; }
    public Func<double, Complex> Ventana { get; }

    public Palabra(
        string texto, 
        double frecuenciaAngular,
        Func<double, Complex> ventana,
        double velocidadGrupo)
        : base(texto, frecuenciaAngular, ventana, velocidadGrupo)
    {
        Texto = texto;
        FrecuenciaAngular = frecuenciaAngular;        
        Fase = t => Complex.FromPolarCoordinates(1.0, frecuenciaAngular * t);
        Ventana = ventana;
    }
}
