using System;
using System.Linq;
using System.Numerics;

public class Designacion : Nombre
{
    public Apariencia Efecto { get; set; }
    public new Palabra Esencia { get; set; }
    public Func<double, Complex> Ventana { get; }
    public Func<double, Complex> Karma { get; }

    /// <summary>
    /// Crea una designación dados su naturaleza, efecto y un factor de atenuación exponencial.
    /// Se genera una ventana multiplicando la atenuación exponencial por la suma de los fasores de la naturaleza.
    /// </summary>
    /// <param name="naturaleza">Nombre asociado a la designación.</param>
    /// <param name="esencia">Esencia asociada a la designación.</param>
    /// <param name="ventana">Función de ventana para la designación.</param>
    /// </summary>
    public Designacion(Nombre naturaleza, Palabra esencia, Func<double, Complex> ventana)
        : base(naturaleza)
    {        
        Efecto = new Apariencia(esencia, naturaleza);
        Esencia = esencia;
        Karma = esencia.Admitancia;
        Ventana = ventana;
    }
    
    /// <summary>
    /// Aparece como una palabra a otra mente dada una frecuencia de respiración.
    /// <param name="frecuenciaRespiracion">Frecuencia de respiración de la otra mente.</param>
    /// </summary>
    public Palabra Aparecer(double frecuenciaRespiracion)
    {
        var palabra = new Palabra(
            Texto,
            Esencia.FrecuenciaAngular + frecuenciaRespiracion,
            t => Ventana(t) * Karma(t)
        );
        return palabra;
    }
}
