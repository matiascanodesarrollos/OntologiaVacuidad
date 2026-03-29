using System;
using System.Text;

public class Nombre : Palabra
{
    public override Guid Id { get; }
    public double Frecuencia { get; internal set; }
    public Apariencia Efecto { get; internal set; }
    public Designacion Esencia { get; internal set; }

    internal Nombre(string texto, 
        double fase, 
        double frecuencia,
        Designacion esencia) 
        : base(texto, fase)
    {
        Id = Guid.NewGuid();
        Esencia = esencia;
        Efecto = new Apariencia(this)
        {
            Causa = this,
        };
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
        Esencia = nombre.Esencia;
    }

    internal Designacion Mostrarse(Nombre nombreProyectado)
    {
        //Modulación PM
        Modular(nombreProyectado);
        //Modulación FM
        Esencia.Modular(nombreProyectado);
        //Modulación AM
        Efecto.Modular(nombreProyectado.Esencia);

        return Esencia;
    }

    /// <summary>
    /// Retorna una representación del nombre.
    /// </summary>
    /// <returns>Naturaleza, fase y frecuencia.</returns>
    public override string ToString() => $"Naturaleza: {(Texto.Length > 20 ? Convert.ToBase64String(Encoding.UTF8.GetBytes(Texto)) : Texto)}, Fase: {Fase * (180 / Math.PI):F2}º, Frecuencia: {Frecuencia:F2} Hz, Amplitud: {Efecto.Amplitud:F2}";

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
