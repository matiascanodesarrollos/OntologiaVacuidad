using System;
using System.Collections.Generic;
using System.Linq;

public class Nombre : Palabra
{
    public Apariencia Esencia { get; internal set; }
    public double Frecuencia { get; private set; }

    /// <summary>
    /// Crea un nuevo nombre con el texto, fase y esencia dados. El Id se genera automáticamente.
    /// </summary>
    /// <param name="texto">El texto del nombre.</param>
    /// <param name="fase">La fase del nombre.</param>
    /// <param name="frecuencia">La frecuencia del nombre.</param>
    /// <param name="esencia">La esencia del nombre. Si es null, se crea una vacuidad.</param>
    public Nombre(string texto, 
        double fase,
        double frecuencia)
        : base(texto, fase, t => frecuencia * t)
    {
        Frecuencia = frecuencia;
        Esencia = new Apariencia(
            t => (Math.Cos(frecuencia * t), Math.Sin(frecuencia * t)), 
            new List<Nombre> { this });
    }

    /// <summary>
    /// Permite la herencia al copiar las propiedades de otro nombre.
    /// Para crear el original usar el metodo estático Aparecer de Apariencia.
    /// </summary>
    /// <param name="nombre">El nombre del cual se copiarán las propiedades.</param>
    public Nombre(Nombre nombre) 
        : base(nombre.Texto, 
            nombre.Fase,
            nombre.FaseInstanea)
    {
        Frecuencia = nombre.Frecuencia;
        Esencia = nombre.Esencia;
    }

    /// <summary>
    /// Crea una nueva designación al proyectar una apariencia sobre un texto.
    /// Cada predicado en el texto se convierte en un nombre con una apariencia asociada.
    /// La frecuencia se determina por la cantidad de nombres que comparten el mismo verbo núcleo (se asume la primer palabra del predicado),
    /// la amplitud por la cantidad de complementos del sujeto que comparten (se asume las palabras restantes del predicado),
    /// y la fase por la posición del predicado en la lista (distribuido en 360º).
    /// </summary>
    /// <param name="apariencia">La apariencia proyectada.</param>
    /// <param name="texto">El texto que funciona como espacio, cada oración se considera un predicado.</param>
    /// <param name="obtenerVerboNucleo">Función que determina el verbo núcleo de un predicado. Si es null, se asume que es la primer palabra.</param>
    /// <returns>La nueva designación creada.</returns>
    public Designacion Mostrarse(Apariencia apariencia, string texto, Func<string, string> obtenerVerboNucleo = null)
    {
        if(obtenerVerboNucleo == null)
        {
            // Se asume que la primer palabra de cada predicado es el verbo núcleo
            obtenerVerboNucleo = predicado => predicado.Split(' ').First(); 
        }
        var designacion = new Designacion(texto, obtenerVerboNucleo);
        var palabra = apariencia.Nombres.SkipLast(1).LastOrDefault() //Derivada
            ?? new Palabra(null, 0, t => 0);
        return designacion.Designar(this, palabra);
    }

    /// <summary>
    /// Representacion en texto de la apariencia. 
    /// </summary>
    /// <returns>Una cadena que representa la apariencia.</returns>
    public override string ToString() => $"{Texto} ({Fase * (180 / Math.PI):F2}º, {Frecuencia:F2} Hz)";

    /// <summary>
    /// Sobreescribe Equals para comparar nombres por su texto y frecuencia.
    /// </summary>
    /// <returns>True si los nombres son iguales, false en caso contrario.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Nombre other)
        {
            return Texto == other.Texto && Frecuencia == other.Frecuencia;
        }
        return false;
    }

    /// <summary>
    /// Sobreescribe GetHashCode para generar el hash code del Id del nombre.
    /// </summary>
    /// <returns>El hash code del nombre.</returns>
    public override int GetHashCode() => Id.GetHashCode();
}
