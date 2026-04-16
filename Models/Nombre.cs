using System;
using System.Collections.Generic;

public class Nombre : Palabra
{
    public override Guid Id { get; }
    public Apariencia Esencia { get; private set; }

    /// <summary>
    /// Crea un nuevo nombre con el texto, fase y esencia dados. El Id se genera automáticamente.
    /// </summary>
    /// <param name="texto">El texto del nombre.</param>
    /// <param name="fase">La fase del nombre.</param>
    /// <param name="esencia">La esencia del nombre. Si es null, se crea una vacuidad.</param>
    public Nombre(string texto, 
        double fase,
        Apariencia esencia)
        : base(texto, fase)
    {
        Id = Guid.NewGuid();
        Esencia = esencia ?? new Apariencia(t => Vacuidad(t));
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
        Id = nombre.Id;
        Esencia = nombre.Esencia;
    }

    /// <summary>
    /// Crea una nueva designación al proyectar el nombre + una lista de predicados sobre la apariencia.
    /// Cada predicado se convierte en un nombre con una apariencia asociada.
    /// La frecuencia se determina por la cantidad de nombres que comparten el mismo verbo núcleo (se asume la primer palabra del predicado),
    /// la amplitud por la cantidad de complementos del sujeto que comparten (se asume las palabras restantes del predicado),
    /// y la fase por la posición del predicado en la lista (distribuido en 360º).
    /// </summary>
    /// <param name="apariencia">La apariencia que funciona como espacio.</param>
    /// <param name="predicados">Los predicados que se utilizarán para crear la nueva designación si la apariencia no es una designación.</param>
    /// <returns>La nueva designación creada.</returns>
    public Designacion Mostrarse(Apariencia apariencia, List<string> predicados)
    {
        var designacion = predicados == null
            ? apariencia as Designacion
            : new Designacion(predicados);

        if (designacion == null)
        {
            throw new ArgumentNullException(nameof(predicados), "Se requieren predicados cuando la apariencia no es una designacion.");
        }

        return designacion.Designar(apariencia, this);
    }

    /// <summary>
    /// Representacion en texto de la apariencia. 
    /// </summary>
    /// <returns>Una cadena que representa la apariencia.</returns>
    public override string ToString() => $"{Texto} ({Fase * (180 / Math.PI):F2}º, {Esencia.Amplitud(1):F2} A)";

    /// <summary>
    /// Sobreescribe Equals para comparar nombres por su Id.
    /// </summary>
    /// <returns>True si los nombres son iguales, false en caso contrario.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Nombre other)
        {
            return Id == other.Id;
        }
        return false;
    }

    /// <summary>
    /// Sobreescribe GetHashCode para comparar nombres por su Id.
    /// </summary>
    /// <returns>El hash code del nombre.</returns>
    public override int GetHashCode() => Id.GetHashCode();
}
