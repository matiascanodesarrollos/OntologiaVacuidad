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
    /// Crea una nueva designación imaginando un nuevo significado para un adjetivo, un sustantivo y un verbo, junto con su fase y frecuencia.
    /// De esta manera se crea una onda portadora que puede ser modulada posteriormente para crear nuevas designaciones a partir de esta.
    /// Se asemeja a la modulación PM [s(φ)=p(φ+m(φ))] en el sentido de crear un "foton" de frecuencia y fase, que luego puede ser modulado por otras designaciones para generar nuevos significados.
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
    /// Crea una nueva designación proyectando un significado existente sobre una apariencia. 
    /// La modulación FM s(f)=p(f+∫m(f)) sobre el significado es un ejemplo de cuando cambiamos el significado de un concepto basado en lo que vemos
    /// La modulación AM s(f)=p(f)*(1+m(f)) sobre la apariencia es un ejemplo de como proyectar un concepto sobre algo puede cambiarlo.
    /// De esta manera se crean nuevas designaciones a partir de otras preexistentes, generando una red de significados interconectados.
    /// La modulación FM simula cómo el nuevo significado puede validar o no el significado proyectado dependiendo de su frecuencia.
    /// La modulación AM simula cómo la nueva designación puede tener más o menos fuerza dependiendo de la apariencia sobre la que se proyecta.
    /// El resultado es una nueva designación que puede ser similar al significado original (si la modulación FM valida el significado proyectado) 
    /// o completamente diferente (si no lo valida), y que puede tener una apariencia más o menos fuerte dependiendo de la modulación AM.
    /// Esta función es fundamental para generar nuevas designaciones a partir de las existentes, permitiendo la evolución y expansión del sistema de designaciones a lo largo del tiempo.
    /// </summary>
    /// <param name="significado">El significado proyectado sobre la apariencia.</param>
    /// <param name="apariencia">La apariencia asociada.</param>
    /// <param name="sustantivo">El sustantivo que define la nueva designación.</param>
    /// <param name="frecuencia">La frecuencia que representa un nuevo significado, por ejemplo cuando un arquitecto diseña una casa</param>
    /// <param name="fase">Simula modulación PM s(φ)=p(φ+m(φ))), que solo puede hacerce conociendo la vacuidad y permite crear nuevas apariencias estables</param>
    /// <returns>La nueva designación creada (con su Nombre/Palabra y su Apariencia).</returns>
    public static Designacion Designar(
        Nombre significado, 
        Apariencia apariencia, 
        string sustantivo, 
        double? frecuencia = null,
        double? fase = null
    )
    {
        var frecuenciaModulada = significado.Modular(apariencia.Significados.Select(c => c.Causa.Frecuencia).ToArray()); //Modulación FM
        apariencia.Modular(significado, frecuencia); //Modulación AM

        var faseModulada = significado.Naturaleza.Fase;
        if(fase.HasValue)
        {
            faseModulada = significado.Naturaleza.Modular(fase.Value); //Modulación PM
        }

        var nuevaDesignacion = Imaginar(significado.Naturaleza.Texto, 
            sustantivo,
            $"Parecer {sustantivo}/{significado.Texto}",
            frecuenciaModulada > 0 ? frecuenciaModulada : apariencia.Esencia.Frecuencia,
            faseModulada);
        nuevaDesignacion.Nombre.Causa = apariencia.Esencia;
        nuevaDesignacion.Nombre.Efecto = nuevaDesignacion.Apariencia;

        return nuevaDesignacion;
    }

    public List<Nombre> BuscarSignificado(int profundidad = 5)
    {
        return Apariencia.Significados.TakeLast(profundidad).ToList();
    }

    public override string ToString()
    {
        var resultado = new StringBuilder();
        resultado.AppendLine("═══ Designación ═══");
        foreach (var causa in Apariencia.Significados)
        {
            if(causa.Causa.Frecuencia == 0)
            {
                break;
            }
            resultado.AppendLine(causa.ToString());
        }
        resultado.AppendLine("═══ Fin ═══");
        return resultado.ToString();
    }

}
