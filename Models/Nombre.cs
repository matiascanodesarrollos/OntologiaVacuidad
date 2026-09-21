using System.Collections.Generic;

public class Nombre : Palabra
{
    public string Sustantivo { get; }
    public string Contexto { get; }
    public Apariencia Esencia { get; }
    internal Dictionary<KeyValuePair<KeyValuePair<double, double>, double>, double> Objeto { get; }

    protected Nombre(Nombre otro)
        : base(otro)
    {
        Sustantivo = otro.Sustantivo;
        Contexto = otro.Contexto;
        Esencia = otro.Esencia;
        Objeto = otro.Objeto;
    }

    /// <summary>
    /// Crea un nuevo nombre con sustantivo, contexto y esfera de admitancia.
    /// </summary>
    /// <param name="sustantivo">Sustantivo para el nombre.</param>
    /// <param name="objeto">Diccionario que representa la convolucion entre la idea expresada en s (Laplace) y su qualia expresada en Fourier.</param>
    /// <param name="contexto">La palabra asociada al nombre.</param>
    public Nombre(string sustantivo, 
        Dictionary<KeyValuePair<KeyValuePair<double, double>, double>, double> objeto,
        Palabra contexto)
        : base(contexto)
    {
        Sustantivo = sustantivo;
        Contexto = contexto.Texto;
        Objeto = objeto;
        Esencia = new Apariencia(this, 0.0);
    }

}
