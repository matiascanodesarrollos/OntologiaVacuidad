using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class Nombre
{
    public string Texto { get; }
    public string Contexto { get; }
    private readonly Lazy<Dictionary<double, Complex>> _fourier;
    public Dictionary<double, Complex> Fourier => _fourier.Value;
    public Func<double, Complex> Ventana { get; }
    public double VelocidadGrupo => CalcularVelocidadGrupo(Texto, Contexto);

    /// <summary>
    /// Crea un nuevo nombre con texto, contexto y su transformada de Fourier.
    /// </summary>
    /// <param name="texto">Texto del nombre.</param>
    /// <param name="contexto">Contexto donde se evaluan apariciones del texto.</param>
    /// <param name="ventana">Función de ventana: debe ponderar la referencia al nombre en cada momento del contexto.</param>
    public Nombre(string texto, 
        string contexto,
        Func<double, Complex> ventana)
    {
        Texto = texto;
        Contexto = contexto;
        Ventana = ventana;
        _fourier = new Lazy<Dictionary<double, Complex>>(CalcularTransformadaFourier);
    }

    /// <summary>
    /// Crea una lista de palabras para expresar el significado del nombre dentro del contexto.
    /// </summary>
    /// <param name="energia">Energía inicial de la idea.</param>
    /// <returns>Una lista de palabras construida a partir del contexto.</returns>
    public IEnumerable<Palabra> Mostrarse(double energia)
    {        
        var mente = new Designacion(Apariencia.Mente(energia), this);
        var palabras = new List<Palabra>()
        {
            new Palabra(Texto, Contexto, 0, Ventana),
        };
        palabras.AddRange(Fourier.Select((kv, i) => new Palabra(
            $"{Texto}_{i}",
            Contexto,
            kv.Key,
            t => 1
        )));
        return palabras;
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

    /// <summary>
    /// Calcula la transformada de Fourier discreta completa de la ventana
    /// usando un paso temporal de 1 por carácter del contexto.
    /// </summary>
    /// <returns>Diccionario de frecuencia angular a valor complejo que representa el espectro.</returns>
    /// <remarks>
    /// Al sobreescribir este método se modifica directamente el espectro almacenado en
    /// <see cref="Fourier"/> y, por tanto, las frecuencias que <see cref="Mostrarse"/> usa
    /// para construir las palabras.
    /// </remarks>
    protected virtual Dictionary<double, Complex> CalcularTransformadaFourier()
    {
        var totalMuestras = Math.Max(1, Contexto.Length);
        var resultado = new (double FrecuenciaAngular, Complex Valor)[totalMuestras];

        for (int k = 0; k < totalMuestras; k++)
        {
            var indiceCentrado = k - (totalMuestras / 2);
            var omega = 2.0 * Math.PI * indiceCentrado / totalMuestras;
            var suma = Complex.Zero;

            for (int n = 0; n < totalMuestras; n++)
            {
                var t = n + 1.0;
                var muestra = Ventana(t);
                suma += muestra * Complex.FromPolarCoordinates(1.0, -omega * t);
            }

            resultado[k] = (omega, suma);
        }

        return resultado.ToDictionary(p => p.FrecuenciaAngular, p => p.Valor);
    }

    /// <summary>
    /// Calcula la velocidad de grupo como la cantidad de apariciones de un texto dentro de su contexto.
    /// </summary>
    /// <param name="texto">Texto objetivo que se busca en el contexto.</param>
    /// <param name="contexto">Contexto donde se evalúan las apariciones.</param>
    /// <returns>Cantidad de apariciones no superpuestas encontradas en el contexto.</returns>
    /// <remarks>
    /// Al sobreescribir este método se modifica directamente el valor expuesto por
    /// <see cref="VelocidadGrupo"/> y, por tanto, la velocidad de grupo que
    /// <see cref="Mostrarse(double)"/> propaga en la apariencia resultante.
    /// </remarks>
    protected virtual int CalcularVelocidadGrupo(string texto, string contexto)
    {
        if (string.IsNullOrEmpty(texto) || string.IsNullOrEmpty(contexto))
        {
            return 0;
        }

        var conteo = 0;
        var indice = 0;

        while (indice < contexto.Length)
        {
            var encontrado = contexto.IndexOf(texto, indice, StringComparison.Ordinal);
            if (encontrado < 0)
            {
                break;
            }

            conteo++;
            indice = encontrado + texto.Length;
        }

        return conteo;
    }
}
