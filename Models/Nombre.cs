using System;
using System.Numerics;

public class Nombre
{
    public string Texto { get; }
    public Func<double, Complex> Ventana { get; }
    public double VelocidadGrupo { get; }

    /// <summary>
    /// Crea un nuevo nombre con texto, velocidad de grupo y su transformada de Fourier.
    /// </summary>
    /// <param name="texto">Texto del nombre.</param>
    /// <param name="velocidadGrupo">Velocidad de propagación asociada al nombre.</param>
    /// <param name="ventana">Función de ventana que devuelve un valor complejo para un tiempo dado.</param>
    public Nombre(string texto, 
        double velocidadGrupo,
        Func<double, Complex> ventana)
    {
        Texto = texto;
        VelocidadGrupo = velocidadGrupo;
        Ventana = ventana;
    }

    /// <summary>
    /// Proyecta esta designación en una nueva apariencia evaluando su función en una frecuencia angular dada.
    /// </summary>
    /// <param name="frecuenciaAngular">Frecuencia angular usada para evaluar la función de la designación.</param>
    /// <returns>Una apariencia construida a partir de la función de esta designación.</returns>
    public Apariencia Mostrarse(double frecuenciaAngular)
    {
        var apariencia = new Apariencia(
            new Palabra(
                Texto,
                frecuenciaAngular,
                Ventana)
        );
        return apariencia;
    }

    /// <summary>
    /// Devuelve una representación textual simple del nombre.
    /// </summary>
    /// <returns>Una cadena con texto y velocidad de grupo.</returns>
    public override string ToString() => $"{Texto} (VelocidadGrupo: {VelocidadGrupo})";

    /// <summary>
    /// Compara nombres por su texto.
    /// </summary>
    /// <returns>True si ambos nombres tienen el mismo texto, false en caso contrario.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Nombre other)
        {
            return Texto == other.Texto;
        }
        return false;
    }

    /// <summary>
    /// Genera el hash code a partir del texto.
    /// </summary>
    /// <returns>El hash code del texto del nombre.</returns>
    public override int GetHashCode() => Texto.GetHashCode();
}
