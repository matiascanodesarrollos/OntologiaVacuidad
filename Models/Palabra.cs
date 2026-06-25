using System;
using System.Numerics;

public class Palabra : Apariencia
{
    public string Texto { get; }
    public Nombre Efecto { get; set; }
    public new Func<double, double, Complex> Funcion { get; }

    public Palabra(
        string texto,
        string contexto,
        double frecuenciaAngularRespiracion,
        Func<double, Complex> admitancia)
        : base(frecuenciaAngularRespiracion)
    {
        Funcion = (tau, t) => 
            Complex.FromPolarCoordinates(1.0, frecuenciaAngularRespiracion * tau)
            * admitancia(t - tau);
        Texto = texto;
        Efecto = new Nombre(
            texto: texto,
            contexto: contexto,
            admitancia: admitancia)
        {
            Causa = this,
        };
        Esencia = new Designacion(
            apariencia: this, 
            nombre: Efecto);
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
        Funcion = (tau, t) => 
            Complex.FromPolarCoordinates(1.0, frecuenciaAngularRespiracion * tau)
            * efecto.Ventana(t - tau);
    }

    internal static Palabra Gozo(double energia) => new Palabra(
        texto: nameof(Gozo),
        contexto: nameof(Nombre.Vacuidad),
        frecuenciaAngularRespiracion: 0.0,
        admitancia: (t) => 
            t < 0
                ? Complex.Zero
                : t == 0
                    ? new Complex(energia, 0.0)
                    : new Complex(0.0, energia / (2 * Math.PI * t))
        );
}
