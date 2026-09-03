using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class Nombre : Palabra
{
    public new string Texto { get; }
    public string Contexto { get; }
    public Apariencia Esencia { get; }
    internal Dictionary<KeyValuePair<Complex, double>, Complex> EsferaAdmitancias { get; }

    protected Nombre(Nombre otro)
        : base(otro)
    {
        Texto = otro.Texto;
        Contexto = otro.Contexto;
        Esencia = otro.Esencia;
        EsferaAdmitancias = otro.EsferaAdmitancias;
    }

    /// <summary>
    /// Crea un nuevo nombre con texto, contexto, mapa de admitancias (s,omega) y esencia.
    /// </summary>
    /// <param name="texto">Texto del nombre.</param>
    /// <param name="esferaAdmitancia">Diccionario que representa una esfera de admitancias para cada s (Laplace) y omega.</param>
    /// <param name="palabra">La palabra asociada al nombre.</param>
    public Nombre(string texto, 
        Dictionary<KeyValuePair<Complex, double>, Complex> esferaAdmitancia,
        Palabra palabra)
        : base(palabra)
    {
        Texto = texto;
        Contexto = palabra.Texto;
        EsferaAdmitancias = esferaAdmitancia;
        Esencia = new Apariencia(palabra, this);
    }

}
