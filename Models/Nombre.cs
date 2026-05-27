using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class Nombre
{
    public Guid Id { get; }
    public string Texto { get; }
    public string Contexto { get; }
    public Func<double, Complex> Ventana { get; }
    public double VelocidadGrupo => CalcularVelocidadGrupo();
    private readonly Lazy<Dictionary<double, Complex>> _fourier;
    public Dictionary<double, Complex> Fourier => _fourier.Value;

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
        Id = Guid.NewGuid();
        Texto = texto;
        Contexto = contexto;
        Ventana = ventana;
        _fourier = new Lazy<Dictionary<double, Complex>>(CalcularTransformadaFourier);
    }

    /// <summary>
    /// Crea una apariencia a partir de un texto.
    /// </summary>
    /// <returns>Una apariencia construida a partir del contexto.</returns>
    public Apariencia Mostrarse(string texto)
    {
        var apariencia = new Palabra(
            texto,
            Contexto,
            Fourier.Sum(p => p.Key),
            Ventana
        );        
        return apariencia;
    }

    /// <summary>
    /// Devuelve una representación textual simple del nombre.
    /// </summary>
    /// <returns>Una cadena con texto y velocidad de grupo.</returns>
    public override string ToString() => $"{Texto} (VelocidadGrupo: {VelocidadGrupo})";

    /// <summary>
    /// Compara nombres por su Id.
    /// </summary>
    /// <returns>True si ambos nombres tienen el mismo Id, false en caso contrario.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Nombre other)
        {
            return Id == other.Id;
        }
        return false;
    }

    /// <summary>
    /// Genera el hash code a partir del Id.
    /// </summary>
    /// <returns>El hash code del Id del nombre.</returns>
    public override int GetHashCode() => Id.GetHashCode();

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
            var omega = 2.0 * Math.PI * k / totalMuestras;
            var suma = Complex.Zero;

            for (int t = 0; t < totalMuestras; t++)
            {
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
    /// <returns>Cantidad de apariciones no superpuestas encontradas en el contexto.</returns>
    /// <remarks>
    /// Al sobreescribir este método se modifica directamente el valor expuesto por
    /// <see cref="VelocidadGrupo"/> y, por tanto, la velocidad de grupo que
    /// <see cref="Mostrarse(double)"/> propaga en la apariencia resultante.
    /// </remarks>
    protected virtual int CalcularVelocidadGrupo()
    {
        if (string.IsNullOrEmpty(Texto) || string.IsNullOrEmpty(Contexto))
        {
            return 0;
        }

        var conteo = 0;
        var indice = 0;

        while (indice < Contexto.Length)
        {
            var encontrado = Contexto.IndexOf(Texto, indice, StringComparison.Ordinal);
            if (encontrado < 0)
            {
                break;
            }

            conteo++;
            indice = encontrado + Texto.Length;
        }

        return conteo;
    }
}
