using System;
using System.Collections.Generic;
using System.Numerics;

public class Palabra : Apariencia
{
    public string Texto { get; }
    public List<Designacion> Efectos { get; } = new List<Designacion>();
    public new Func<double, double, Complex> Funcion { get; }

    public Palabra(
        string texto,
        Func<double, Complex> admitancia,
        Designacion efecto)
        : base(efecto.FrecuenciaAngular)
    {
        Funcion = (tau, t) => 
            Complex.FromPolarCoordinates(1.0, efecto.FrecuenciaAngular * tau)
            * admitancia(t - tau);
        Texto = texto;
        Efectos.Add(efecto);
        Esencia = efecto;
    }

    internal Palabra(string texto, Designacion efecto, Apariencia apariencia)
        : base(apariencia.Funcion)
    {
        Texto = texto;
        Efectos.Add(efecto);        
        Funcion = (tau, t) => 
            Complex.FromPolarCoordinates(1.0, efecto.FrecuenciaAngular * tau)
            * efecto.Ventana(t - tau);
    }

    internal static Palabra Gozo(double energia) 
    {
        var nombre = Nombre.Vacuidad(
            contexto: nameof(Gozo),
            conductancia: 0.0,
            susceptancia: energia);
        var designacion = new Designacion(
            sTFT: (omega, tau) => energia,
            nombre: nombre);
        var palabra = new Palabra(
            texto: nameof(Gozo),
            admitancia: (t) => 
                t < 0
                    ? Complex.Zero
                    : t == 0
                        ? new Complex(energia, 0.0)
                        : new Complex(0.0, energia / (2 * Math.PI * t)),
            efecto: designacion)
            {
                Esencia = designacion,
            };
        designacion.Causa = palabra;
        return palabra;
    }
}
