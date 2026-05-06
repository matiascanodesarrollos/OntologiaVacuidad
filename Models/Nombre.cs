using System;
using System.Collections.Generic;

public class Nombre : Palabra
{
    public Designacion Causa { get; }
    public Apariencia Esencia => new Apariencia(Causa);
    public double Frecuencia { get; }
    public double Amplitud { get; }

    /// <summary>
    /// Crea un nuevo nombre con el texto, fase y frecuencia dados. 
    /// La esencia o apariencia se calcula como una exponencial compleja con la frecuencia dada.
    /// </summary>
    /// <param name="texto">El texto del nombre.</param>
    /// <param name="fase">La fase del nombre.</param>
    /// <param name="frecuencia">La frecuencia del nombre.</param>
    internal Nombre(string texto, 
        double fase,
        double frecuencia,
        double amplitud,
        Designacion causa)
        : base(texto, fase)
    {
        Frecuencia = frecuencia;
        Amplitud = amplitud;
        Causa = causa;
    }

    /// <summary>
    /// Funcion para crear un nombre y su apariencia. Análogo a imaginar.
    /// </summary>
    /// <param name="texto">Texto o palabra asociada.</param>
    /// <param name="fase">Fase del nuevo nombre.</param>
    /// <param name="frecuencia">Frecuencia principal.</param>
    /// <param name="amplitud">Amplitud deseada.</param>
    /// <returns>Un nuevo nombre</returns>
    public static Nombre Imaginar(
        string texto,
        double fase,
        double frecuencia,
        double amplitud)
    {
        var nombre = new Nombre(
            texto,
            fase,
            frecuencia,
            amplitud,
            Designacion.Vacuidad
        );
        return nombre;
    }

    /// <summary>
    /// Permite la herencia al copiar las propiedades de otro nombre.
    /// Para crear el original usar el metodo estático Aparecer de Apariencia.
    /// </summary>
    /// <param name="nombre">El nombre del cual se copiarán las propiedades.</param>
    public Nombre(Nombre nombre) 
        : base(nombre.Texto, 
            nombre.Fase)
    {
        Frecuencia = nombre.Frecuencia;
        Amplitud = nombre.Amplitud;
        Causa = nombre.Causa;
    }

    /// <summary>
    /// Crea una nueva designación al proyectar una apariencia sobre un texto.
    /// Cada predicado en el texto se convierte en un nombre con una apariencia asociada.
    /// La frecuencia se determina por la cantidad de nombres que comparten el mismo verbo núcleo (se asume la primer palabra del predicado),
    /// la amplitud por la cantidad de complementos del sujeto que comparten (se asume las palabras restantes del predicado),
    /// y la fase por la posición del predicado en la lista (distribuido en 360º).
    /// </summary>
    /// <param name="texto">El texto que funciona como espacio, cada oración se considera un predicado.</param>
    /// <param name="mapeoNombres">Función que determina el mapeo de nombres a partir del texto .</param>
    /// <returns>La nueva designación creada.</returns>
    public Designacion Mostrarse(string texto, 
        Func<string, Dictionary<double, List<Nombre>>> mapeoNombres)
    {
        var designacion = new Designacion(
            texto, 
            mapeoNombres);
        return Designacion.Designar(this, new Apariencia(designacion));
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

    /// <summary>
    /// Crea un nuevo nombre con el texto "Vacuidad", fase 0, frecuencia 0 y la amplitud dada, asociado a la causa Vacuidad.
    /// </summary>
    /// <param name="amplitud">La amplitud del nuevo nombre.</param>
    /// <param name="designacion">La designación asociada al nuevo nombre.</param>
    /// <returns>Un nuevo nombre asociado a la causa Vacuidad.</returns>
    public static Nombre Cuerpo(double amplitud, Designacion designacion) => new Nombre(nameof(Designacion.Vacuidad), 0, 0, amplitud, designacion);
}
