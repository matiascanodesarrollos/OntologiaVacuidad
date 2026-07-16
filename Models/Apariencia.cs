using System;
using System.Numerics;

public class Apariencia
{ 
    public Func<double, Complex> Funcion { get; }
    public Lazy<Complex> Fasor { get; }
    public double FrecuenciaAngular { get; }
    public Nombre Esencia { get; set; }

    internal Apariencia(double frecuenciaAngular)
    {
        FrecuenciaAngular = frecuenciaAngular;
        Fasor = new Lazy<Complex>(() => 
            Esencia.CalcularFourier(FrecuenciaAngular)
        );
        Funcion = t => 
            Fasor.Value 
            * Complex.FromPolarCoordinates(1, FrecuenciaAngular * t);
    }
}

