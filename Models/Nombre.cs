using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class Nombre
{
    public Guid Id { get; }
    public string Texto { get; }
    public string Contexto { get; }
    public Apariencia Esencia { get; }
    public Func<double, Complex> Ventana { get; }
    internal double FrecuenciaAngular { get; }
    internal Palabra Causa { get; set; }

    protected Nombre(Nombre otro)
    {
        Id = otro.Id;
        Texto = otro.Texto;
        Contexto = otro.Contexto;
        Ventana = otro.Ventana;
        Esencia = otro.Esencia;
        Causa = otro.Causa;
        FrecuenciaAngular = otro.FrecuenciaAngular;
    }

    /// <summary>
    /// Crea un nuevo nombre con texto, contexto y su transformada de Fourier.
    /// </summary>
    /// <param name="texto">Texto del nombre.</param>
    /// <param name="contexto">Contexto donde se evaluan apariciones del texto.</param>
    /// <param name="admitancia">Función de ventana: debe ponderar la referencia al nombre en cada momento del contexto.</param>
    public Nombre(string texto, 
        string contexto,
        Func<double, Complex> admitancia)
    {
        Id = Guid.NewGuid();
        Texto = texto;
        Contexto = contexto;
        Ventana = admitancia;
        FrecuenciaAngular = EstimarFrecuencias().Keys.Sum();
        Esencia = new Apariencia(FrecuenciaAngular);
        Esencia.Esencia = new Designacion(Esencia, this);
    }

    internal static Nombre Vacuidad(
        string contexto,
        double flujoRespiracion, 
        double intencionControl) => new Nombre(
            nameof(Vacuidad),
            contexto,
            t => new Complex(flujoRespiracion, intencionControl));

    /// <summary>
    /// Crea una designación para la palabra y el nombre. 
    /// Usa la STFT para crear la apariencia resultante.
    /// </summary>
    /// <param name="palabra">Palabra que se desea mostrar.</param>
    /// <returns>La apariencia construida.</returns>
    public Apariencia Mostrarse(Palabra palabra)
    {
        var designacion = new Designacion(palabra, this);
        var apariencia = new Apariencia(
            tau => designacion.STFT(FrecuenciaAngular, tau))
        {
            Esencia = designacion,
        };
        return apariencia;
    }    

    /// <summary>
    /// Calcula la transformada de Fourier de la ventana.
    /// Sobreescribir para definir otro criterio.
    /// </summary>
    /// <param name="omega">Frecuencia angular de análisis.</param>
    /// <returns>El integral complejo de la ventana.</returns>
    public virtual Complex Fourier(double omega)
    {
        var muestras = 100;
        var periodoMuestreo = 0.01;
        var integral = Complex.Zero;

        for (var n = 0; n < muestras; n++)
        {
            var t = n * periodoMuestreo;
            var muestra = Ventana(t);
            var factor = Complex.FromPolarCoordinates(1.0, -omega * t);
            integral += muestra * factor;
        }

        return integral;
    }

    /// <summary>
    /// Calcula la transformada de Fourier discreta completa de la ventana
    /// usando un paso temporal de 1 por carácter del contexto y saltos de 100 en la frecuencia angular hasta 30000.
    /// Sobreescribir para definir otro criterio de estimación de frecuencias.
    /// </summary>
    public virtual Dictionary<double, Complex> EstimarFrecuencias()
    {
        var muestras = 100;
        var frecuenciaMaxima = 5000;
        var deltaFrecuencia = 100;
        var periodoMuestreo = 0.01;
        var resultado = new Dictionary<double, Complex>();

        for(var omega = 0.0; omega <= frecuenciaMaxima; omega += deltaFrecuencia)
        {         
            var suma = Complex.Zero;            
            for (int n = 0; n < muestras; n++)
            {
                var t = n * periodoMuestreo;
                var muestra = Ventana(t);
                var factor = Complex.FromPolarCoordinates(1, -omega * t);
                suma += muestra * factor;
            }

            if(suma.Magnitude > 1e-6)
            {
                resultado.Add(omega, suma);
            }            
        }

        return resultado;
    }

    /// <summary>
    /// Devuelve una representación textual simple del nombre.
    /// </summary>
    /// <returns>Una cadena con texto y velocidad de grupo.</returns>
    public override string ToString() => $"{Texto}";

    /// <summary>
    /// Compara nombres por su Id.
    /// </summary>
    /// <returns>True si ambos nombres tienen el mismo Id, false en caso contrario.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Nombre other)
        {
            return Id == other.Id;
        }
        return false;
    }

    /// <summary>
    /// Genera el hash code a partir del Id.
    /// </summary>
    /// <returns>El hash code del Id del nombre.</returns>
    public override int GetHashCode() => Id.GetHashCode();
}
