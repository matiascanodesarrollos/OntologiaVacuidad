using System;
using System.Linq;
using System.Text;

public class Designacion
{
    public Guid Id { get; }    
    public string Texto { get; }
    public Nombre Nombre { get; private set; }
    public Apariencia Apariencia { get; private set; }    
    public double Frecuencia { get; internal set; }

    private Designacion(string verbo, double frecuencia)
    {
        Id = Guid.NewGuid();
        Texto = verbo;        
        Frecuencia = frecuencia;
    }
    
    internal Apariencia Modular(
        Nombre significado, 
        Apariencia apariencia, 
        string sujeto,
        string predicado
    )
    {
        //Modulación FM, simula la integral del mensaje instantáneo
        var frecuenciaModulada = significado.Causa.Frecuencia;
        foreach (var frecuenciaNaturaleza in apariencia.Naturalezas.Select(c => c.Causa.Frecuencia))
        {
            if(Math.Abs(Frecuencia - frecuenciaNaturaleza) <= 1)
            {
                frecuenciaModulada = frecuenciaNaturaleza; 
            }
        }
        
        //Creo la nueva naturaleza
        var caracterEspacio = ' ';
        var predicadoArray = predicado.Split(caracterEspacio);
        var verbo = predicadoArray.First();
        var adjetivo = string.Join(caracterEspacio, predicadoArray.Skip(1));
        return Crear(sujeto, verbo, adjetivo, frecuenciaModulada, significado.Naturaleza.Fase).Apariencia;
    }


    /// <summary>
    /// Crea una esencia o nueva designación imaginando un nuevo significado para un adjetivo, un sustantivo y un verbo, junto con su fase y frecuencia.
    /// De esta manera se crea una onda portadora que puede ser modulada posteriormente para crear nuevas designaciones a partir de esta.
    /// Se asemeja a la modulación PM [s(φ)=p(φ+m(φ))] en el sentido de crear un "foton" de frecuencia y fase, que luego puede ser modulado por otras designaciones para generar nuevos significados.
    /// </summary>
    /// <param name="sustantivo">Permite derivar la frecuencia</param>
    /// <param name="verbo">Permite derivar la amplitud</param>
    /// <param name="adjetivo">Permite derivar la fase</param>
    /// <param name="frecuencia">Permite controlar la interacción con otras ondas y por lo tanto el significado</param>
    /// <param name="fase">Permite elegir la función de la onda portadora (ej: cos(φ)=sen(φ+90º))</param>
    /// <returns>La nueva designación creada (con su Nombre/Palabra y su Apariencia).</returns>
    public static Designacion Crear(
        string sustantivo, 
        string verbo, 
        string adjetivo,
        double frecuencia,
        double fase)
    {
        var designacion = new Designacion(verbo, frecuencia);
        var nuevaPalabra = new Palabra(adjetivo, fase);
        var nuevoNombre = new Nombre(sustantivo, nuevaPalabra);
        designacion.Apariencia = nuevoNombre.Mostrarse(designacion);
        designacion.Nombre = nuevoNombre;
        return designacion;
    }

    public override string ToString()
    {
        var resultado = new StringBuilder();
        resultado.AppendLine("═══ Designación ═══");
        foreach (var causa in Apariencia.Naturalezas)
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
