using System;
using System.Text;

public class Designacion
{
    public Guid Id { get; }    
    public string Verbo { get; }
    public Nombre Nombre { get; private set; }
    public Apariencia Apariencia { get; private set; }    
    public double Frecuencia { get; internal set; }

    private Designacion(Nombre nombre, Apariencia apariencia, string verbo, double frecuencia)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        Apariencia = apariencia;
        Verbo = verbo;        
        Frecuencia = frecuencia;
    }


    /// <summary>
    /// Crea una nueva designación imaginando un nuevo significado a partir de un adjetivo, un sustantivo y un verbo, junto con su fase y frecuencia.
    /// De esta manera se crea una onda portadora que puede ser modulada posteriormente para crear nuevas designaciones a partir de esta.
    /// Se asemeja a la modulación PM [s(φ)=p(φ+m(φ))].
    /// </summary>
    /// <param name="adjetivo">Permite derivar la fase</param>
    /// <param name="nombre">Permite derivar la frecuencia</param>
    /// <param name="verbo">Permite derivar la amplitud</param>
    /// <param name="frecuencia">Permite controlar la interacción con otras ondas y por lo tanto el significado</param>
    /// <param name="fase">Permite elegir la función de la onda portadora (ej: cos(φ)=sen(φ+90º))</param>
    /// <returns>La nueva designación creada (con su Nombre/Palabra y su Apariencia).</returns>
    public static Designacion Imaginar(
        string adjetivo, 
        string nombre, 
        string verbo,
        double frecuencia,
        double fase)
    {
        var designacion = new Designacion(null, null, verbo, frecuencia);
        var nuevoNombre = new Nombre(nombre, new Palabra(adjetivo, fase), null);
        var nuevaApariencia = new Apariencia(designacion, nuevoNombre);

        designacion.Nombre = nuevoNombre;
        designacion.Apariencia = nuevaApariencia;

        nuevoNombre.Causa = designacion;
        nuevoNombre.Efecto = nuevaApariencia;

        return designacion;
    }

    /// <summary>
    /// Crea una nueva designación proyectando un significado existente sobre una apariencia
    /// </summary>
    /// <param name="significado">El significado proyectado sobre la apariencia.</param>
    /// <param name="apariencia">La apariencia asociada al significado base.</param>
    /// <param name="sustantivo">El sustantivo que define la nueva designación.</param>
    /// <param name="frecuencia">Permite modular estilo FM[s(f)=p(f+∫m(f))]</param>
    /// <param name="fase">Permite modular estilo AM[s(φ)=p(φ)*(1+m(φ)))]</param>
    /// <returns>La nueva designación creada (con su Nombre/Palabra y su Apariencia).</returns>
    public static Designacion Designar(
        Nombre significado, 
        Apariencia apariencia, 
        string sustantivo, 
        double? frecuencia = null, //Designar buscando crear un nuevo significado, por ejemplo cuando un arquitecto diseña una casa.
        double? fase = null //Designar conociendo la vacuidad, se controla por completo como interactua la nueva designación con la base y por lo tanto su apariencia.
    )
    {
        var frecuenciaModulada = apariencia.Esencia.Frecuencia + 1; //Se asume una frecuencia muy cercana para que las ondas interactuen.
        if (frecuencia.HasValue)
        {
            frecuenciaModulada = significado.Causa.Frecuencia * frecuencia.Value; // ∫m(f)df ≈ m(f) * Δf (aproximación de la integral)
        }

        var faseModulada = !fase.HasValue 
            ? apariencia.Causa.Naturaleza.Fase + Math.PI / 2 //Se asume desfase de 90º para evitar interferencia y permitir que interactuen.
            : apariencia.Causa.Naturaleza.Fase * (1 + fase.Value); // Modulación AM
        faseModulada %= 2 * Math.PI;

        var nuevaDesignacion = Imaginar(significado.Naturaleza.Texto, 
            sustantivo,
            $"Parecer {sustantivo}/{significado.Causa.Verbo}",
            frecuenciaModulada,
            faseModulada);
        nuevaDesignacion.Nombre.Causa = apariencia.Esencia;
        nuevaDesignacion.Nombre.Efecto = nuevaDesignacion.Apariencia;

        return nuevaDesignacion;
    }

    public override string ToString()
    {
        var resultado = new StringBuilder();
        resultado.AppendLine("═══ Designación ═══");
        var apariencias = Nombre.BuscarSignificado();
        foreach (var apariencia in apariencias)
        {
            resultado.AppendLine(apariencia.ToString());
        }
        return resultado.ToString();
    }

}
