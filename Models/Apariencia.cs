using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class Apariencia : Palabra
{
    private readonly Lazy<Complex> nombre;
    public double Amplitud => nombre.Value.Magnitude;
    public Func<double, Complex> Funcion => t => nombre.Value * Fase(t);
    public Designacion Esencia { get; }

    /// <summary>
    /// Crea una copia pública de otra apariencia preservando su comportamiento para herencia.
    /// /// <param name="otra">La apariencia de la cual se copiarán las propiedades.</param>
    /// </summary>
    public Apariencia(Apariencia otra)
        : base(otra.Texto, otra.FrecuenciaAngular, otra.Ventana)
    {
        nombre = new Lazy<Complex>(() => otra.nombre.Value);
        Esencia = otra.Esencia;
    }
    

    internal Apariencia(Palabra palabra)
        : base(palabra.Texto, palabra.FrecuenciaAngular, palabra.Ventana)
    {
        nombre = new Lazy<Complex>(() => CalcularTransformadaFourier(FrecuenciaAngular));
        Esencia = new Designacion(
            new Nombre(palabra.Texto, palabra.FrecuenciaAngular, palabra.Ventana),
            this);
    }

    internal Apariencia(Palabra palabra, Designacion esencia)
        : base(palabra.Texto, palabra.FrecuenciaAngular, palabra.Ventana)
    {
        nombre = new Lazy<Complex>(() => CalcularTransformadaFourier(FrecuenciaAngular));
        Esencia = esencia;
    }

    /// <summary>
    /// Crea una apariencia compuesta a partir de una lista de palabras.
    /// </summary>
    /// <param name="palabras">Palabras de entrada que se combinan en una sola señal.</param>
    /// <returns>Una apariencia cuya ventana es la productoria de fase por ventana de cada palabra.</returns>
    public static Apariencia Aparecer(IEnumerable<Palabra> palabras)
    {
        var lista = palabras.ToList();
        var palabra = new Palabra(
            string.Join(" ", lista.Select(p => p.Texto)),
            lista.Sum(p => p.FrecuenciaAngular),
            t => lista.Aggregate(
                Complex.One,
                (acc, p) => acc * (p.Fase(t) * p.Ventana(t))
            )
        );
        return new Apariencia(palabra);
    }


    /// <summary>
    /// Sobreescribe GetHashCode para comparar apariencias por su Id.
    /// </summary>
    /// <returns>El hash code de la apariencia.</returns>
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    /// Sobreescribe Equals para comparar apariencias por su Id.
    /// </summary>
    /// <returns>True si las apariencias son iguales, false en caso contrario.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Apariencia other)
        {
            return Id == other.Id;
        }
        return false;
    }

    public static Apariencia Mente => new Apariencia(
        new Palabra(
            nameof(Mente),
            Designacion.Vacuidad.FrecuenciaAngular,
            t => 1.0 / (2 * Math.PI) //Transformada inversa de δ(ω)
        )
    );

    /// <summary>
    /// Calcula la transformada de Fourier compleja de <see cref="Ventana"/> para una frecuencia dada.
    /// </summary>
    /// <param name="frecuenciaAngular">Frecuencia angular en la que se evalúa el espectro.</param>
    /// <returns>Valor complejo de la transformada de Fourier en la frecuencia indicada.</returns>
    /// <remarks>
    /// Al sobreescribir este método se modifica directamente el valor interno usado por
    /// <see cref="Amplitud"/> y <see cref="Funcion"/>. La implementación derivada debería conservar
    /// estabilidad numérica y devolver valores finitos para evitar propagar NaN o infinito.
    /// </remarks>
    protected virtual Complex CalcularTransformadaFourier(double frecuenciaAngular)
    {
        const double limiteIntegracion = 8.0;
        const int pasos = 4096;
        var dt = (2.0 * limiteIntegracion) / pasos;

        var suma = Complex.Zero;

        for (var i = 0; i <= pasos; i++)
        {
            var t = -limiteIntegracion + (i * dt);
            var ventana = Ventana(t);
            var peso = (i == 0 || i == pasos) ? 0.5 : 1.0;

            var exponente = Complex.FromPolarCoordinates(1.0, -frecuenciaAngular * t);
            suma += peso * ventana * exponente;
        }

        return suma * dt;
    }
}

