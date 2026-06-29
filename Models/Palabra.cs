using System;
using System.Collections.Generic;
using System.Numerics;

public class Palabra : Apariencia
{
    public string Texto { get; }
    public List<Designacion> Efectos { get; } = new List<Designacion>();
    public new Func<double, double, Complex> Funcion { get; }

    internal Palabra(string texto, Designacion efecto, Apariencia apariencia)
        : base(apariencia.Funcion)
    {
        Texto = texto;
        Efectos.Add(efecto);        
        Funcion = (tau, t) => 
            Complex.FromPolarCoordinates(1.0, efecto.FrecuenciaAngular * tau)
            * efecto.Ventana(t - tau);
    }

    internal Palabra(
        string texto, 
        Func<double, double, Complex> funcion, 
        Func<double, Complex> funcionApariencia)
        : base(funcionApariencia)
    {
        Texto = texto;
        Funcion = funcion;
    }
}
