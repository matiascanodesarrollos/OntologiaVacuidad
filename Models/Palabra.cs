using System;
using System.Collections.Generic;
using System.Numerics;

public class Palabra : Apariencia
{
    public string Texto { get; }
    public Func<double, Complex> Admitancia { get; } 
    public Designacion Efecto { get; set; }

    /// <summary>
    /// Crea una palabra y su apariencia correspondiente.
    /// La función de la palabra modela la respiración:
    /// Devuelve la presión en la parte real y el flujo de aire en la imaginaria.
    /// </summary>
    /// <param name="texto">Texto que se dijo.</param>
    /// <param name="contexto">Contexto en el que se pronuncia la palabra.</param>
    /// <param name="frecuenciaAngular">Frecuencia angular respiratoria.</param>
    /// <param name="admitancia">Función de admitancia que modifica los componentes de la respiración.</param>
    public Palabra(
        string texto,
        string contexto,
        double frecuenciaAngular,
        Func<double, Complex> admitancia,
        Dictionary<double, Complex> fourier)
        : base(frecuenciaAngular)
    {
        Texto = texto;
        Admitancia = admitancia;
        Esencia = new Nombre(
            texto: texto,
            contexto: contexto,
            fourier: fourier,
            esencia: this
        );
    }

    /// <summary>
    /// Aproxima la onda reflejada y transmitida por la palabra, se hace una aproximación basada en 
    /// la amplitud instantanea (asumiendo siempre el mismo ángulo de incidencia).
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
        var admitancia = Efecto.STFT(
            this, 
            tau,
            omega);       
        
        // Coeficiente de reflexión (aproximado).
        var numerador = ondaIncidente.Magnitude - admitancia.Magnitude;
        var denominador = ondaIncidente.Magnitude + admitancia.Magnitude;
        var gamma = numerador / denominador;

        var ondaReflejada = gamma * ondaIncidente;
        var ondaTransmitida = admitancia * ondaIncidente;
        return (ondaReflejada, ondaTransmitida);
    }
}
