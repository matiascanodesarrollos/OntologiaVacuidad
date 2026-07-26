using System;
using System.Collections.Generic;
using System.Numerics;

public class Apariencia : Palabra
{ 
    public Func<double, Complex> Funcion { get; }
    public Complex Fasor { get; }
    public double FrecuenciaAngular { get; }
    public Nombre Esencia { get; set; }
    public Designacion Efecto { get; set; }

    /// <summary>
    /// Crea una apariencia con texto, contexto, admitancia, frecuencia angular y transformada de Fourier.
    /// <param name="texto">Texto de la apariencia.</param>
    /// <param name="contexto">Contexto en el que se pronuncia la apariencia.</param>
    /// <param name="admitancia">Función de admitancia para esa frecuencia.</param>
    /// <param name="frecuenciaAngular">Frecuencia angular respiratoria.</param>
    /// <param name="fourier">Transformada de Fourier de la apariencia.</param>
    /// </summary>
    public Apariencia(
        string texto, 
        string contexto,
        Func<double, Complex> admitancia,
        double frecuenciaAngular,
        Dictionary<double, Complex> fourier) 
        : base(texto, admitancia)
    {
        FrecuenciaAngular = frecuenciaAngular;
        Esencia = new Nombre(
            texto: texto,
            contexto: contexto,
            fourier: fourier,
            esencia: this
        );
        Fasor = Esencia.CalcularFourier(FrecuenciaAngular);
        Funcion = t => 
            Fasor 
            * Complex.FromPolarCoordinates(1, FrecuenciaAngular * t);
    }

    /// <summary>
    /// Calcula la onda reflejada y transmitida por la palabra.
    /// Sobre escribir para definir otro criterio.
    /// </summary>
    /// <param name="tau">Tiempo de la designación.</param>
    /// <param name="t">Tiempo de la apariencia.</param>
    /// <param name="omega">Frecuencia angular de la designación.</param>
    /// <returns>El valor de la onda.</returns>
    public virtual (Complex ondaReflejada, Complex ondaTransmitida) Aparecer(
        double tau,
        double t, 
        double omega)
    {
        var ondaIncidente = Funcion(t);

        var Y1 = Admitancia(t - tau);
        var Y2 = Efecto.STFT(
            this, 
            tau,
            omega);        
        var numerador = Y2 - Y1;
        var denominador = Y2 + Y1;
        if (denominador == Complex.Zero)
        {
            return (ondaIncidente, Complex.Zero);
        }
        var gamma = numerador / denominador;
        
        var ondaReflejada = gamma * ondaIncidente;
        var ondaTransmitida = ondaIncidente + ondaReflejada;
        return (ondaReflejada, ondaTransmitida);
    }
}

