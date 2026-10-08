using System;
using System.Numerics;

public class Designacion : Nombre
{
    public Apariencia Efecto { get; set; }
    public new Palabra Esencia { get; set; }
    public Func<double, Complex> Ventana { get; }

    /// <summary>
    /// Crea una designación dados su naturaleza, efecto y un factor de atenuación exponencial.
    /// Se genera una ventana multiplicando la atenuación exponencial por la suma de los fasores de la naturaleza.
    /// </summary>
    /// <param name="esencia">Palabra asociada a la designación.</param>
    /// <param name="naturaleza">Nombre asociado a la designación.</param>
    /// <param name="ventana">Función de ventana espacial.</param>
    /// <param name="sigma">Factor de atenuación exponencial.</param>
    /// <returns>Una nueva instancia de Designacion.</returns>
    public Designacion(
        Palabra esencia,
        Nombre naturaleza, 
        Func<double, Complex> ventana,
        double sigma,
        double x)
        : base(naturaleza)
    {
        Ventana = ventana;
        Efecto = new Apariencia(naturaleza, (k, t) => Ventana(k - x) * Complex.Exp(-sigma * t));
        Esencia = esencia;
    }
}
