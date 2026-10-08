using System;
using System.Collections.Generic;
using System.Numerics;

public class Nombre : Palabra
{
    public string Sustantivo { get; }
    public string Contexto { get; }
    public Apariencia Esencia { get; }
    internal Func<double, KeyValuePair<double, double>, Complex> Funcion { get; }

    protected Nombre(Nombre otro)
        : base(otro)
    {
        Sustantivo = otro.Sustantivo;
        Contexto = otro.Contexto;
        Esencia = otro.Esencia;
        Funcion = otro.Funcion;
    }

    /// <summary>
    /// Crea un nuevo nombre con sustantivo, contexto y esfera de admitancia.
    /// </summary>
    /// <param name="sustantivo">Sustantivo para el nombre.</param>
    /// <param name="objeto">Diccionario que representa la convolucion entre la idea expresada en s (Laplace) y su qualia expresada en Fourier.</param>
    /// <param name="contexto">La palabra asociada al nombre.</param>
    /// <param name="funcionTransferencia">Función que representa la transferencia entre el espacio de frecuencias y el espacio-tiempo.</param>
    public Nombre(string sustantivo, 
        Dictionary<KeyValuePair<double, KeyValuePair<double, double>>, Complex> objeto,
        Palabra contexto,        
        Func<double, double, Complex> funcionTransferencia)
        : base(contexto)
    {
        Sustantivo = sustantivo;
        Contexto = contexto.Texto;
        Funcion = (omega, s) => {
            var coordenadaEsferica = new KeyValuePair<double, KeyValuePair<double, double>>(omega, s);
            return objeto[coordenadaEsferica];
        };
        Esencia = new Apariencia(
            this, 
            (k, t) => 
                Complex.Exp(Complex.ImaginaryOne * contexto.FrecuenciaAngular * k) 
                * funcionTransferencia(k, t));
    }

}
