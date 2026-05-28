using System;
using System.Numerics;

public class Palabra : Apariencia
{
    public new string Texto { get; }
    public new Func<double, double, Complex> Funcion { get; }
    
    
    /// <summary>
    /// Crea una palabra a partir de un texto, un nombre y una frecuencia angular.
    /// La función de la palabra se construye multiplicando la función de ventana del nombre por la fase compleja correspondiente.
    /// La causa de la apariencia resultante o su naturaleza es la palabra misma.
    /// </summary>
    /// <param name="texto">Texto de la palabra.</param>
    /// <param name="nombre">Nombre que aporta la ventana de análisis y el contexto.</param>
    /// <param name="frecuenciaAngular">Frecuencia angular de análisis.</param>
    /// <returns>Una nueva palabra vinculada al nombre de entrada.</returns>
    public Palabra(
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
    /// Calcula una nueva apariencia a partir de esta palabra y el tiempo de pensamiento tau en el concepto o nombre.
    /// La frecuencia angular de la nueva apariencia se obtiene de la fase de la función evaluada.
    /// </summary>
    /// <param name="t">Tiempo basado en el texto de la palabra.</param>
    /// <param name="tau">Tiempo del pensamiento.</param>
    /// <returns>Una nueva apariencia de ventana constante igual a la transformada Z de su esencia.</returns>
    public Apariencia Aparecer(double t, double tau)
    {
        var z = Funcion(tau, t);
        var muestras = Math.Max(1, Contexto.Length);
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
            Texto, 
            Contexto, 
            t => new Complex(X.Magnitude, omega * t - X.Phase),
            velocidadGrupo);
        var apariencia = new Apariencia(
            nombre,
            omega
        );
        return apariencia;
    }
}
