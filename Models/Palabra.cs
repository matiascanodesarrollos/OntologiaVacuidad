using System;
using System.Numerics;

public class Palabra : Apariencia
{
    public string Texto { get; }
    public Nombre Efecto { get; set; }
    internal new Func<double, Complex> Funcion { get; }

    public Palabra(
        string texto,
        string contexto,
        double frecuenciaAngularRespiracion,
        double tiempo,
        Func<double, Complex> admitancia)
        : base(frecuenciaAngularRespiracion)
    {
        Funcion = tau => 
            Complex.FromPolarCoordinates(1.0, frecuenciaAngularRespiracion * tau)
            * admitancia(tiempo - tau);
        Texto = texto;
        Efecto = new Nombre(
            texto,
            contexto,
            admitancia)
        {
            Causa = this,
        };
        Esencia = new Designacion(this, Efecto);
    }

    internal Palabra(string texto, Nombre efecto, Apariencia apariencia)
        : base(apariencia.Funcion)
    {
        Texto = texto;
        Efecto = efecto;
        var deltaT = 0.01;
        var muestraUno = apariencia.Funcion(0.0);
        var muestraDos = apariencia.Funcion(deltaT);
        var division = muestraDos / muestraUno;
        var frecuenciaAngularRespiracion = division.Phase / deltaT;
        Funcion = tau => 
            Complex.FromPolarCoordinates(1.0, frecuenciaAngularRespiracion * tau)
            * efecto.Admitancia(0.0 - tau);
    }

    internal static Palabra Gozo(double energia) => new Palabra(
        nameof(Gozo),
        nameof(Nombre.Vacuidad),
        0.0,
        0.0,
        (t) => 
            Complex.FromPolarCoordinates(1.0, energia) 
            * (t <= 0
                ? new Complex(0.5 * energia, 0.0)
                : new Complex(0.0, energia / (2 * Math.PI * t)))
        );    
}
