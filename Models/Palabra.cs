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
    /// Calcula la imagen mental de la palabra.
    /// Se utiliza una aproximación de las impedancias basada en el valor temporal de las ondas (consideradas en el calculo de la STFT).
    /// Sobre escribir para definir otro criterio.
    /// </summary>
    /// <param name="tauPalabra">Tiempo de la palabra.</param>
    /// <param name="t">Tiempo de la apariencia.</param>
    /// <param name="omegaDesignacion">Frecuencia angular de la designación.</param>
    /// <param name="tauDesignacion">Tiempo de la designación.</param>
    /// <returns>El valor de la onda.</returns>
    public virtual Complex Aparecer(
        double tauPalabra, 
        double t, 
        double omegaDesignacion, 
        double tauDesignacion)
    {
        var ondaTransmitida = Funcion(
            tauPalabra, 
            t);
        var ondaIncidente = Efecto.STFT(
            this, 
            tauDesignacion,
            omegaDesignacion);
        
        // Coeficiente de reflexión complejo (aproximado).
        var denominador = ondaIncidente + ondaTransmitida;
        if (denominador.Magnitude < 1e-12)
        {
            return Complex.Zero;
        }
        var gamma = (ondaIncidente - ondaTransmitida) / denominador;
        
        var ondaReflejada = gamma * ondaTransmitida;
        return ondaReflejada + ondaIncidente;
    }
}
