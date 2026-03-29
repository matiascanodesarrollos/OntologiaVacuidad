using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainLogic.Services
{
    public static class AmbienteConfig
    {
        public static Designacion CrearAmbiente(string texto = null)
        {
            var frecuenciaBase = 1000;
            if (!string.IsNullOrEmpty(texto))
            {
                var oraciones = texto
                    .Split('.')
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrEmpty(t))
                    .ToList();
                return Designacion.Designar(oraciones, frecuenciaBase);
            }
            
            var designacion = Designacion.Designar(new List<string>(), frecuenciaBase);
            var apariencia = designacion.Aparecer();
            return apariencia;
        }
    }
}
