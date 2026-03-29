using System.Collections.Generic;
using System.Linq;

namespace DomainLogic.Services
{
    public static class AmbienteConfig
    {
        public static Designacion CrearAmbiente(string texto = null)
        {
            if (!string.IsNullOrEmpty(texto))
            {
                var oraciones = texto.Split('.').ToList();
                var designaciones = oraciones                    
                    .Select(p => new Designacion(new List<string> { p }))
                    .ToList();
                var resultado = designaciones.First();
                foreach(var oracion in designaciones.Skip(1))
                {
                    resultado.Causa.Mostrarse(oracion);
                }
                return resultado;
            }
            
            var designacion = new Designacion(new List<string>())
                .Aparecer(new List<string> { 
                    "Ser mente luminosa",
                    "Parecer vasto espacio",
                    "Estar",
                    "Mostrar apariencia" });
            var apariencia = designacion
                .Aparecer(new List<string> {
                    "Aparecer agua pura", 
                    "Aparecer tierra pura y sólida",
                    "Aparecer aire vibración pura",
                    "Aparecer fuego",
            });
            return apariencia;
        }
    }
}
