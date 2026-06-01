using System;
using System.Numerics;

public class Palabra : Apariencia
{
    public new string Texto { get; }
    public new Func<double, double, Complex> Funcion { get; }
    
    internal Palabra(
        string texto,
        Nombre nombre,
        double frecuenciaAngular)
        : base(nombre, frecuenciaAngular)
    {
        Texto = texto;
        Funcion = (tau, t) => 
            Complex.FromPolarCoordinates(1.0, frecuenciaAngular * tau) 
            * nombre.Ventana(t - tau);
        Causa = this;
    }

    /// <summary>
    /// Crea una designación evaluando la STFT en un punto complejo z (representa la persona que pregunta).
    /// </summary>
    /// <param name="z">Punto complejo para evaluar la STFT.</param>
    /// <param name="respuesta">Como se expresa el concepto a esa persona.</param>
    /// <returns>Una nueva designación vinculada a la palabra.</returns>
    public Designacion Aparecer(Complex z, string respuesta)
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
            respuesta, 
            Texto, 
            t => t > 0 ? X.Magnitude : 0.0,
            velocidadGrupo);
        var apariencia = new Apariencia(nombre, X.Phase)
        {
            Causa = this
        };
        return apariencia.Esencia;
    }
}
