using System.Collections.Generic;
using System.Numerics;

public class Nombre
{
    public string Texto { get; }
    public string Contexto { get; }
    public Apariencia Esencia { get; }
    internal Dictionary<KeyValuePair<Complex, double>, Complex> MapaAdmitancias { get; }

    protected Nombre(Nombre otro)
    {
        Texto = otro.Texto;
        Contexto = otro.Contexto;        
        Esencia = otro.Esencia;
        MapaAdmitancias = otro.MapaAdmitancias;
    }

    /// <summary>
    /// Crea un nuevo nombre con texto, contexto, mapa de admitancias (s,omega) y esencia.
    /// </summary>
    /// <param name="texto">Texto del nombre.</param>
    /// <param name="contexto">Contexto de la designación.</param>
    /// <param name="admitancia">Diccionario de frecuencias y sus correspondientes admitancias como valores complejos.</param>
    /// <param name="esencia">Apariencia asociada al nombre.</param>
    public Nombre(string texto, 
        string contexto,
        Dictionary<KeyValuePair<Complex, double>, Complex> admitancia,
        Palabra esencia)
    {
        Texto = texto;
        Contexto = contexto;
        MapaAdmitancias = admitancia;
        Esencia = new Apariencia(esencia, this);
    }

}
