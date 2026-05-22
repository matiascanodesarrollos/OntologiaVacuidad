using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class Apariencia
{
    public Guid Id { get; }    
    public Func<double, Complex> Funcion { get; }
    public Designacion Esencia { get; }
    public double Amplitud => _nombre.Value.Magnitude;
    private readonly Lazy<Complex> _nombre; // Es lazy porque en general aprendemos el nombre en el futuro
    

    /// <summary>
    /// Crea una copia pública de otra apariencia preservando su comportamiento para herencia.
    /// /// <param name="otra">La apariencia de la cual se copiarán las propiedades.</param>
    /// </summary>
    public Apariencia(Apariencia otra)
    {
        Id = otra.Id;        
        Funcion = otra.Funcion;
        Esencia = otra.Esencia;
        _nombre = new Lazy<Complex>(() => otra._nombre.Value);
    }

    internal Apariencia(string texto, double frecuenciaAngular, Func<double, Complex> ventana)
    {
        Id = Guid.NewGuid();
        Funcion = t => Amplitud * Complex.FromPolarCoordinates(1.0, frecuenciaAngular * t);
        var palabra = this as Palabra;
        Esencia = new Designacion(this, new Nombre(texto, palabra.CalcularVelocidadGrupo(frecuenciaAngular), ventana));        
        _nombre = new Lazy<Complex>(() => CalcularTransformadaFourier(frecuenciaAngular, ventana));
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
                (acc, p) => acc * (p.Fase(t) * p.Nombre.Ventana(t))
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
            0.0,
            t => new Complex(
                t == 0.0 ? 0.5 * double.PositiveInfinity : 0.0, 
                1 / (2 * Math.PI * t)) //Transformada inversa de u(ω)
        )
    );

    /// <summary>
    /// Calcula la transformada de Fourier compleja para una frecuencia dada.
    /// </summary>
    /// <param name="frecuenciaAngular">Frecuencia angular en la que se evalúa el espectro.</param>
    /// <param name="funcion">Función que se transforma.</param>
    /// <returns>Valor complejo de la transformada de Fourier en la frecuencia indicada.</returns>
    /// <remarks>
    /// Al sobreescribir este método se modifica directamente el valor interno usado por
    /// <see cref="Amplitud"/>. La implementación derivada debería conservar
    /// estabilidad numérica y devolver valores finitos para evitar propagar NaN o infinito.
    /// </remarks>
    protected virtual Complex CalcularTransformadaFourier(double frecuenciaAngular, Func<double, Complex> funcion)
    {
        const double limiteIntegracion = 8.0;
        const int pasos = 4096;
        var dt = 2.0 * limiteIntegracion / pasos;

        var suma = Complex.Zero;

        for (var i = 0; i <= pasos; i++)
        {
            var t = -limiteIntegracion + (i * dt);
            var valor = funcion(t);
            var peso = (i == 0 || i == pasos) ? 0.5 : 1.0;

            var exponente = Complex.FromPolarCoordinates(1.0, -frecuenciaAngular * t);
            suma += peso * valor * exponente;
        }

        return suma * dt;
    }
}

