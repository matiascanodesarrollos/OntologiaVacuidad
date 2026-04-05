using System;
using System.Collections.Generic;

public class Nombre : Palabra
{
    public override Guid Id { get; }
    public double Frecuencia { get; internal set; }
    public Apariencia Efecto { get; internal set; }

    internal Nombre(string texto, 
        double fase, 
        double frecuencia) 
        : base(texto, fase)
    {
        Id = Guid.NewGuid();
        Efecto = new Designacion(new List<Nombre>(){ this, });
        Frecuencia = frecuencia;
    }

    /// <summary>
    /// Permite la herencia al copiar las propiedades de otro nombre.
    /// Para crear el original usar el metodo estático Designar de Designacion.
    /// </summary>
    /// <param name="nombre">El nombre del cual se copiarán las propiedades.</param>
    public Nombre(Nombre nombre) 
        : base(nombre.Texto, 
            nombre.Fase)
    {
        Id = nombre.Id;
        Efecto = nombre.Efecto;
        Frecuencia = nombre.Frecuencia;
    }

    /// <summary>
    /// Permite mostrar la designación en un espacio de apariencia, proyectando sus propiedades en ese espacio.
    /// </summary>
    /// <param name="espacio">El espacio de apariencia donde se mostrará la designación.</param>
    /// <param name="funcionProyeccion">Función opcional para proyectar la designación en el espacio.</param>
    /// <returns>La designación proyectada en el espacio.</returns>
    public Designacion Mostrarse(Designacion espacio)
    {
        espacio.Nombres.Add(this);
        var designacion = new Designacion(espacio.Nombres);
        return designacion;
    }

    /// <summary>
    /// Retorna una representación del nombre.
    /// </summary>
    /// <returns>Naturaleza, fase y frecuencia.</returns>
    public override string ToString() => $"{Texto} ({Fase * (180 / Math.PI):F2}º, {Frecuencia:F2} Hz, {Efecto.Amplitud:F2} A)";

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
