using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class Apariencia
{
    public Guid Id { get; }    
    public Func<double, Complex> Funcion { get; }
    public Designacion Esencia { get; }
    public Lazy<double> Amplitud { get; } 

    /// <summary>
    /// Crea una copia pública de otra apariencia preservando su comportamiento para herencia.
    /// /// <param name="otra">La apariencia de la cual se copiarán las propiedades.</param>
    /// </summary>
    public Apariencia(Apariencia otra)
    {
        Id = otra.Id;        
        Funcion = otra.Funcion;
        Esencia = otra.Esencia;
        Amplitud = otra.Amplitud;
    }

    internal Apariencia(
        string texto, 
        double frecuenciaAngular, 
        Func<double, Complex> ventana, 
        double velocidadGrupo)
    {
        Id = Guid.NewGuid();
        var nombre = new Nombre(
                texto, 
                velocidadGrupo, 
                ventana);
        Amplitud = new Lazy<double>(() => 
            nombre.Fourier.ContainsKey(frecuenciaAngular) 
                ? nombre.Fourier[frecuenciaAngular].Magnitude 
                : 1.0);
        Funcion = t => Amplitud.Value * Complex.FromPolarCoordinates(1.0, frecuenciaAngular * t);
        var palabra = this as Palabra;
        Esencia = new Designacion(
            this, 
            nombre);        
    }

    /// <summary>
    /// Crea una apariencia compuesta a partir de una lista de palabras.
    /// </summary>
    /// <param name="palabras">Palabras de entrada que se combinan en una sola señal.</param>
    /// <param name="velocidadGrupo">Velocidad de grupo que se asigna a la apariencia resultante.</param>
    /// <returns>Una apariencia cuya ventana es la productoria de fase por ventana de cada palabra.</returns>
    public static Apariencia Aparecer(IEnumerable<Palabra> palabras, double velocidadGrupo)
    {
        var lista = palabras.ToList();
        var palabra = new Palabra(
            string.Join(" ", lista.Select(p => p.Texto)),
            lista.Sum(p => p.FrecuenciaAngular),
            t => lista.Aggregate(
                Complex.One,
                (acc, p) => acc * (p.Fase(t) * p.Ventana(t))
            ),
            velocidadGrupo
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

    public static Apariencia Mente(double energia) => new Apariencia(
        new Palabra(
            nameof(Mente),
            0.0,
            t => new Complex(
                t == 0.0 ? 0.5 * energia : 0.0, 
                energia / (2 * Math.PI * t)),
            0.0
        ) //Transformada inversa de u(ω)
    );
}

