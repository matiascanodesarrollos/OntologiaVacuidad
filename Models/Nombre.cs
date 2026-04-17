using System;
using System.Collections.Generic;

public class Nombre : Palabra
{
    public Apariencia Esencia { get; private set; }
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
            this);
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
    /// Crea una nueva designación al proyectar una apariencia sobre una lista de predicados.
    /// Cada predicado se convierte en un nombre con una apariencia asociada.
    /// La frecuencia se determina por la cantidad de nombres que comparten el mismo verbo núcleo (se asume la primer palabra del predicado),
    /// la amplitud por la cantidad de complementos del sujeto que comparten (se asume las palabras restantes del predicado),
    /// y la fase por la posición del predicado en la lista (distribuido en 360º).
    /// </summary>
    /// <param name="apariencia">La apariencia proyectada.</param>
    /// <param name="predicados">La lista de predicados que funciona como espacio.</param>
    /// <returns>La nueva designación creada.</returns>
    public Designacion Mostrarse(Apariencia apariencia, List<string> predicados)
    {
        var designacion = new Designacion(predicados);
        return designacion.Designar(this, apariencia.Esencia);
    }

    /// <summary>
    /// Representacion en texto de la apariencia. 
    /// </summary>
    /// <returns>Una cadena que representa la apariencia.</returns>
    public override string ToString() => $"{Texto} ({Fase * (180 / Math.PI):F2}º, {Frecuencia:F2} Hz)";

    /// <summary>
    /// Sobreescribe Equals para comparar nombres por su Id.
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
    /// Sobreescribe GetHashCode para comparar nombres por su Id.
    /// </summary>
    /// <returns>El hash code del nombre.</returns>
    public override int GetHashCode() => Id.GetHashCode();
}
