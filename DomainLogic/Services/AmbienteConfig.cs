using System;
using System.Linq;

namespace DomainLogic.Services
{
    public static class AmbienteConfig
    {
        /// <summary>
        /// Se divide el texto en oraciones utilizando el punto como delimitador. 
        /// Cada oración se considera un predicado que se convertirá en un nombre dentro de la designación del ambiente.
        /// Se asume que el verbo núcleo de cada oración es la primera palabra, 
        /// y los complementos del sujeto son las palabras restantes.
        /// La frecuencia de cada nombre se determina por la cantidad de nombres que comparten el mismo verbo núcleo.
        /// La amplitud se determina por la cantidad de complementos del sujeto que comparten.
        /// La fase se asigna de manera equidistante.
        /// </summary>
        /// <param name="texto">El texto que se utilizará para crear la designación del ambiente.</param>
        /// <returns>Una designación que representa el ambiente creado a partir del texto.</returns>
        public static Designacion CrearAmbiente(string texto)
        {         
            var nombre = Nombre.Imaginar(
                0, 
                0, texto.Split('.')
                    .Select(p => p.Trim())
                    .Count(p => !string.IsNullOrEmpty(p)));
            return nombre.Mostrarse(texto);
        }
    }

    
}
