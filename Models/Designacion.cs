using System;
using System.Linq;
using System.Text;

public class Designacion
{
    public Guid Id { get; }    
    public string Texto { get; internal set; }
    public Nombre Esencia { get; private set; }
    public Apariencia Naturaleza { get; private set; }    
    public double Frecuencia { get; internal set; }
    public double AnchoBanda { get; set; } = 2;

    private Designacion(string verbo, double frecuencia)
    {
        Id = Guid.NewGuid();
        Texto = verbo;
        Frecuencia = frecuencia;
    }

    private void Actualizar(string sujeto, string predicado, double frecuencia)
    {
        Frecuencia = frecuencia;        
        var delimitador = ' ';
        var predicadoArray = predicado.Split(delimitador);        
        Texto = predicadoArray.First();
        Esencia.Naturaleza.Texto = string.Join(delimitador, predicadoArray.Skip(1));
        Esencia.Texto = sujeto;        
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
        designacion.Naturaleza = nuevoNombre.Mostrarse(designacion, $"{verbo} {adjetivo}");
        designacion.Esencia = nuevoNombre;
        return designacion;
    }
    
    /// <summary>
    /// Cambia la naturaleza de la designación si el nombre proyectado tiene una frecuencia que pueda incluirse dentro del ancho de banda.
    /// Se produce algo similar a la modulación FM.
    /// Sobreescribir para un comportamiento mas detallado.
    /// </summary>
    /// <param name="significado">El nombre proyectado que se utilizará para modular la designación.</param>
    /// <param name="predicado">El predicado que se utilizará para actualizar la designación.</param>
    /// <returns>La nueva frecuencia de la designación después de la modulación.</returns>
    public virtual double Modular(
        Nombre significado,
        string predicado
    )
    {        
        //Modulación FM, simula la integral del mensaje instantáneo
        var frecuenciaModulada = Frecuencia;        
        foreach (var naturaleza in Naturaleza.Efectos)
        {
            if(Math.Abs(significado.Causa.Frecuencia - naturaleza.Causa.Frecuencia) <= AnchoBanda)
            {
                frecuenciaModulada = naturaleza.Causa.Frecuencia;
                naturaleza.Causa.Actualizar(significado.Texto, predicado, frecuenciaModulada);
            }
        }
        Actualizar(significado.Texto, predicado, frecuenciaModulada);
        return Frecuencia;
    }    

    /// <summary>
    /// Sobreescribe ToString para mostrar una representación de la designación, incluyendo su naturaleza y esencia.
    /// Se muestra la naturaleza como una lista de efectos, cada uno con su causa, frecuencia y fase.
    /// </summary>
    /// <returns>Una cadena que representa la designación.</returns>
    public override string ToString()
    {
        var resultado = new StringBuilder();
        resultado.AppendLine("═══ Designación ═══");
        foreach (var causa in Naturaleza.Efectos)
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
