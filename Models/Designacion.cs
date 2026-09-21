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
    /// <param name="naturaleza">Nombre asociado a la designación.</param>
    /// <param name="ventana">Función de ventana espacial.</param>
    /// <param name="radio">Radio del punto en la esfera del nombre, es igual la velocidad de la apariencia asociada.</param>
    /// <returns>Una nueva instancia de Designacion.</returns>
    public Designacion(
        Nombre naturaleza, 
        Func<double, Complex> ventana,
        double radio)
        : base(naturaleza)
    {
        Ventana = ventana;
        var palabra = new Palabra(
            Sustantivo,
            0.0,
            x => (Ventana(x + radio) - Ventana(x)) / radio //W'(x)
        );
        Efecto = new Apariencia(naturaleza, radio);
        Esencia = palabra;
    }
}
