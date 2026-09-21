using System;
using System.Collections.Generic;
using System.Numerics;

public class Apariencia
{
    public Func<double, double, Complex> Funcion { get; }
    public Nombre Esencia { get; set; }

    /// <summary>
    /// Crea una apariencia a partir de el nombre.
    /// La velocidad de propagación se utiliza para calcular la función de la apariencia.
    /// </summary>
    /// <param name="esencia">Nombre de la apariencia.</param>
    /// <param name="velocidad">Velocidad de propagación.</param>
    public Apariencia(Nombre esencia, double velocidad) 
    {
        Esencia = esencia;
        Funcion = (k, t) =>
        {
            var omega = k * velocidad;
            var frecuenciaIdea = new KeyValuePair<double, double>(0, omega);
            var coordenadaEsferica = new KeyValuePair<KeyValuePair<double, double>, double>(frecuenciaIdea, omega);
            return Esencia.Objeto[coordenadaEsferica]
            * Complex.FromPolarCoordinates(1, omega * t);
        };
    }
}

