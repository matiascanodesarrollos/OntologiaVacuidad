using System;
using System.Numerics;

public class Palabra : Apariencia
{
    public string Texto { get; }    
    public new Func<double, double, Complex> Funcion { get; }
    public Designacion Esencia { get; }
    public Apariencia Efecto { get; internal set; }

    private Palabra(
        string texto,
        Func<double, double, Complex> funcion,
        double energia)
        : base(
            0, 
            t => t > 0 
                ? new Complex(0.5 * energia, energia / (2 * Math.PI * t)) 
                : Complex.One, 
            energia)
    {
        Texto = texto;
        Funcion = funcion;
    }

    public static Palabra Gozo(double energia) => new Palabra(
        "gozo",
        (tau, t) => 
            Complex.FromPolarCoordinates(1.0, energia) 
            * (t <= 0
                ? new Complex(0.5 * energia, 0.0)
                : new Complex(0.0, energia / (2 * Math.PI * t))),
        energia);

    internal Palabra(
        string texto,
        Designacion designacion)
        : base(designacion)
    {
        Texto = texto;
        Funcion = (tau, t) => 
            Complex.FromPolarCoordinates(1.0, FrecuenciaAngular * tau) 
            * designacion.Ventana(t - tau);
        Causa = new Lazy<Palabra>(() => this);
        Efecto = this;
    }

    /// <summary>
    /// Crea una apariencia evaluando la STFT en un punto complejo z (representa la persona que pregunta).
    /// La nueva apariencia se vuelve el efecto de esta palabra.
    /// </summary>
    /// <param name="z">Punto complejo para evaluar la STFT.</param>
    /// <param name="texto">Texto que se usará para nombrar la nueva apariencia.</param>
    /// <returns>Una nueva apariencia cuya causa es la palabra.</returns>
    public Apariencia Aparecer(Complex z, string texto)
    {
        var muestras = Math.Max(1, Esencia.Contexto.Length);
        var omega = z.Phase;
        var paso = 0.01;
        var X = Complex.Zero;
        var derivada = Complex.Zero;
        for (int n = 0; n < muestras; n++)
        {
            var factor = Complex.Pow(z, -n);
            var valor = Esencia.STFT(n, omega);
            X += valor * factor;
            derivada += (Esencia.STFT(n, omega + paso) - Esencia.STFT(n, omega - paso)) * factor / (2.0 * paso);
        }
        var velocidadGrupo = X.Magnitude <= 1e-12 ? 0.0 : (derivada / X).Imaginary;
        var nombre = new Nombre(
            texto,
            Esencia.Texto,
            t => t > 0 ? X : Complex.Zero,
            velocidadGrupo);
        var apariencia = new Apariencia(X.Phase, nombre.Ventana, X.Magnitude);
        Efecto = apariencia;
        return Efecto;
    }
}
