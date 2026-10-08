using System;
using System.Numerics;

public class Apariencia
{
    public Func<double, double, Complex> Funcion { get; }
    public Nombre Esencia { get; set; }

    /// <summary>
    /// Crea una apariencia a partir del nombre y una función que describe su comportamiento.
    /// La función toma como parámetros la frecuencia espacial y la frecuencia temporal.
    /// </summary>
    /// <param name="esencia">Nombre de la apariencia.</param>
    /// <param name="funcion">Función que describe el comportamiento de la apariencia en función de la frecuencia espacial y temporal.</param>
    public Apariencia(Nombre esencia, Func<double, double, Complex> funcion) 
    {
        Esencia = esencia;
        Funcion = funcion;
    }
}

