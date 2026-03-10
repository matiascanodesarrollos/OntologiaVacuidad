using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Designacion
{
    public Guid Id { get; }    
    public string Texto { get; }
    public Nombre Nombre { get; private set; }
    public Apariencia Apariencia { get; private set; }    
    public double Frecuencia { get; internal set; }

    private Designacion(Nombre nombre, Apariencia apariencia, string verbo, double frecuencia)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        Apariencia = apariencia;
        Texto = verbo;        
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
    /// Crea una nueva designación proyectando un significado existente sobre una apariencia, simula modulación FM s(f)=p(f+∫m(f))
    /// </summary>
    /// <param name="significado">El significado proyectado sobre la apariencia.</param>
    /// <param name="apariencia">La apariencia asociada al significado base.</param>
    /// <param name="sustantivo">El sustantivo que define la nueva designación.</param>
    /// <param name="frecuencia">Simula modulación AM s(f)=p(f)*(1+m(f)))</param>
    /// <param name="fase">Simula modulación PM s(φ)=p(φ+m(φ)))</param>
    /// <returns>La nueva designación creada (con su Nombre/Palabra y su Apariencia).</returns>
    public static Designacion Designar(
        Nombre significado, 
        Apariencia apariencia, 
        string sustantivo, 
        double? frecuencia = null, //Designar buscando crear un nuevo significado, por ejemplo cuando un arquitecto diseña una casa.
        double? fase = null //Designar conociendo la vacuidad
    )
    {
        var frecuenciaModulada = significado.Modular(apariencia.Causas.Select(c => c.Causa.Frecuencia).ToArray()); //Modulación FM s(f)=p(f+∫m(f))
        apariencia.Modular(significado, frecuencia); //Modulación AM s(f)=p(f)*(1+m(f)))

        var faseModulada = significado.Naturaleza.Fase;
        if(fase.HasValue)
        {
            faseModulada = significado.Naturaleza.Modular(fase.Value);
        }

        var nuevaDesignacion = Imaginar(significado.Naturaleza.Texto, 
            sustantivo,
            $"Parecer {sustantivo}/{significado.Causa.Texto}",
            frecuenciaModulada > 0 ? frecuenciaModulada : apariencia.Esencia.Frecuencia,
            faseModulada);
        nuevaDesignacion.Nombre.Causa = apariencia.Esencia;
        nuevaDesignacion.Nombre.Efecto = nuevaDesignacion.Apariencia;

        return nuevaDesignacion;
    }

    public List<Nombre> BuscarSignificado(int profundidad = 5)
    {
        return Apariencia.Causas.TakeLast(profundidad).ToList();
    }

    public override string ToString()
    {
        var resultado = new StringBuilder();
        resultado.AppendLine("═══ Designación ═══");
        foreach (var apariencia in Apariencia.Causas)
        {
            if(apariencia.Causa.Frecuencia == 0)
            {
                break;
            }
            resultado.AppendLine(apariencia.ToString());
        }
        resultado.AppendLine("═══ Fin ═══");
        return resultado.ToString();
    }

}
