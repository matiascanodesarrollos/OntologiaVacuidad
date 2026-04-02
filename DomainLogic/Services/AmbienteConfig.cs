using System.Linq;

namespace DomainLogic.Services
{
    public static class AmbienteConfig
    {
        public static Designacion CrearAmbiente(string texto)
        {
            var oraciones = texto
                    .Split('.')
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrEmpty(t))
                    .ToList();
                
                return Apariencia
                    .Aparecer(oraciones)
                    as Designacion;
        }
    }
}
