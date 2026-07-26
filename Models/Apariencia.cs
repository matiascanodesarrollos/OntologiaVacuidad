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
    /// <param name="texto">Descripción de la apariencia.</param>
    /// <param name="contexto">Descripción del contexto en el que ocurre la apariencia.</param>
    /// <param name="admitancia">Función de admitancia para la frecuencia angular portadora.</param>
    /// <param name="frecuenciaAngularPortadora">Frecuencia angular respiratoria portadora.</param>
    /// <param name="frecuenciaAdmitancia">Diccionario de frecuencias y sus correspondientes admitancias como valores complejos.</param>
    /// </summary>
    public Apariencia(
        string texto, 
        string contexto,
        Func<double, Complex> admitancia,
        double frecuenciaAngularPortadora,
        Dictionary<double, Complex> frecuenciaAdmitancia) 
        : base(texto, admitancia)
    {
        FrecuenciaAngular = frecuenciaAngularPortadora;
        Esencia = new Nombre(
            texto: texto,
            contexto: contexto,
            frecuenciaAdmitancia: frecuenciaAdmitancia,
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

