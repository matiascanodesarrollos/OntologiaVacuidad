using System;
using System.Collections.Generic;
using System.Numerics;

public class Palabra : Apariencia
{
    public string Texto { get; }
    public Func<double, Complex> Admitancia { get; }
    public new Func<double, double, Complex> Funcion { get; internal set; }
    public Designacion Efecto { get; set; }

    /// <summary>
    /// Crea una palabra y su apariencia correspondiente.
    /// La función de la palabra modela la respiración:
    /// Devuelve la presión en la parte real y el flujo de aire en la imaginaria.
    /// </summary>
    /// <param name="texto">Texto que se dijo.</param>
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
        Funcion = (tau, t) => 
            Complex.FromPolarCoordinates(1, frecuenciaAngular * tau)
            * Admitancia(t - tau);
        Esencia = new Nombre(
            texto: texto,
            contexto: contexto,
            fourier: fourier,
            esencia: this
        );
    }
    
    /// <summary>
    /// Calcula la onda reflejada y transmitida de la palabra.
    /// Sobre escribir para definir otro criterio.
    /// </summary>
    /// <param name="tauPalabra">Tiempo de la palabra.</param>
    /// <param name="t">Tiempo de la apariencia.</param>
    /// <returns>El valor de la onda.</returns>
    public virtual (Complex ondaReflejada, Complex ondaTransmitida) Aparecer(
        double tauPalabra, 
        double t)
    {
        var apariencia = this as Apariencia;
        var ondaIncidente = apariencia.Funcion(t);
        var Y1 = Admitancia(t - tauPalabra);
        var Y2 = Efecto.CalcularFourier(FrecuenciaAngular);

        var denominador = Y1 + Y2;
        if (denominador.Magnitude < 1e-10)
        {
            return (Complex.Zero, ondaIncidente);
        }
        var coeficienteDeTransmision = 2 * Y1 / denominador;
        var coeficienteReflexion = (Y1 - Y2) / denominador;
        
        var ondaReflejada = coeficienteReflexion * ondaIncidente;
        var ondaTransmitida = coeficienteDeTransmision * ondaIncidente;        
        return (ondaReflejada, ondaTransmitida);
    }
}
