using System.Collections.Generic;
using System.Numerics;

public class Nombre : Palabra
{
    public string Sustantivo { get; }
    public string Contexto { get; }
    public Apariencia Esencia { get; }
    internal Dictionary<KeyValuePair<Complex, double>, Complex> Significado { get; }

    protected Nombre(Nombre otro)
        : base(otro)
    {
        Sustantivo = otro.Sustantivo;
        Contexto = otro.Contexto;
        Esencia = otro.Esencia;
        Significado = otro.Significado;
    }

    /// <summary>
    /// Crea un nuevo nombre con sustantivo, contexto e imagen mental o significado.
    /// </summary>
    /// <param name="sustantivo">Sustantivo para el nombre.</param>
    /// <param name="imagenMental">Diccionario que representa una esfera de admitancias para cada s (Laplace) y omega.</param>
    /// <param name="contexto">La palabra asociada al nombre.</param>
    public Nombre(string sustantivo, 
        Dictionary<KeyValuePair<Complex, double>, Complex> imagenMental,
        Palabra contexto)
        : base(contexto)
    {
        Sustantivo = sustantivo;
        Contexto = contexto.Texto;
        Significado = imagenMental;
        Esencia = new Apariencia(contexto, this);
    }

}
