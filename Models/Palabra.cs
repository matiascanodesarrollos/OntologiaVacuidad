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
    /// Calcula la onda o palabra reflejada por la designación dada la apariencia.
    /// </summary>
    /// <param name="tauPalabra">Tiempo de la palabra.</param>
    /// <param name="t">Tiempo de la apariencia.</param>
    /// <param name="omegaDesignacion">Frecuencia angular de la designación.</param>
    /// <param name="tauDesignacion">Tiempo de la designación.</param>
    /// <returns>Un número complejo.</returns>
    public Complex Aparecer(
        double tauPalabra, 
        double t, 
        double omegaDesignacion, 
        double tauDesignacion)
    {
        var z1 = Funcion(
            tauPalabra, 
            t);
        var z2 = Efecto.STFT(
            this, 
            omegaDesignacion, 
            tauDesignacion);     

        //Reflexion de la palabra sobre la designación, calculando el coeficiente de reflexión.
        var a1 = z1.Magnitude;
        var a2 = z2.Magnitude;
        var numerador = a2 - a1;
        var denominador = a2 + a1;
        if (denominador == 0)
        {
            return Complex.Zero;
        }
        var gamma = numerador / denominador;

        return gamma * z1;
    }
}
