using System;
using System.Linq;

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
        Efecto = new Apariencia(this);
        Frecuencia = frecuencia;
        Esencia = esencia;
    }

    /// <summary>
    /// Permite la herencia al copiar las propiedades de otro nombre.
    /// Para crear el original usar el metodo estático Crear de Designacion.
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

    /// <summary>
    /// Se muestra como el nombre proyectado, afectando la apariencia .
    /// Se produce algo similar a la modulación QAM (PM + AM) y FM en secuencia sobre la instancia.
    /// Sobreescribir los metodos Modular de Palabra y Apariencia para un comportamiento mas detallado.
    /// Los mismo se llaman en esa secuencia.
    /// </summary>
    /// <param name="designacionProyectada">La designación que se utilizará para mostrar la apariencia.</param>
    /// <returns>La apariencia resultante</returns>
    public Designacion Mostrarse(Designacion designacionProyectada)
    {
        //Modulación PM
        Modular(designacionProyectada.Causa);
        //Modulación FM
        var nombrePrincipal = designacionProyectada
            .Nombres
            .OrderByDescending(n => n.Efecto.Amplitud)
            .First();
        Esencia.Modular(nombrePrincipal);
        //Modulación AM
        Efecto.Modular(designacionProyectada);

        return designacionProyectada;
    }

    /// <summary>
    /// Retorna una representación del nombre.
    /// </summary>
    /// <returns>Naturaleza, fase y frecuencia.</returns>
    public override string ToString() => $"Naturaleza: {Texto}, Fase: {Fase * (180 / Math.PI):F2}º, Frecuencia: {Frecuencia:F2} Hz, Amplitud: {Efecto.Amplitud:F2}";
        
    /// <summary>
    /// Sobreescribe GetHashCode para comparar nombres por su Id.
    /// </summary>
    /// <returns>El hash code del nombre.</returns>
    public override int GetHashCode() => Id.GetHashCode();

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
}
