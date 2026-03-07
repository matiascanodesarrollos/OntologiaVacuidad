using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DomainLogic.Services
{
    public static class AmbienteConfig
    {
        public static List<Designacion> PilotosOfdmFrame { get; private set; } = new List<Designacion>();
        public static List<Designacion> SubPilotosOfdmFrame { get; private set; } = new List<Designacion>();
        public static Designacion CrearAmbiente(ILogger logger)
        {
            logger.LogInformation("═══ Configurando ambiente ═══\n");
            var frecuenciaBase = 100000.0; //Frecuencia base para el espacio, se asume que es la más alta para que las demás interactuen con ella.  
            var frecuenciaExtrema = frecuenciaBase * 2;
            var frecuenciaAlta = frecuenciaBase/100;
            var frecuenciaMediaAlta = frecuenciaBase/1000;
            var frecuenciaMediaBaja = frecuenciaBase/5000;
            var frecuenciaBaja = frecuenciaBase/10000;

            var espacio = Designacion.Imaginar("Vacío", "Espacio", "Estar", 0, frecuenciaBase);
            var tiempo = Designacion.Imaginar("Puro", "Tiempo", "Ser", 2 * Math.PI, 0);   
            var tierraPura = Designacion.Designar(espacio.Nombre, tiempo.Apariencia, "TierraPura", frecuenciaAlta, 0);
            var aguaPura = Designacion.Designar(tierraPura.Nombre, tierraPura.Apariencia, "AguaPura", frecuenciaBase);
            var airePuro = Designacion.Designar(aguaPura.Nombre, aguaPura.Apariencia, "AirePuro", frecuenciaMediaBaja);
            var fuegoPuro = Designacion.Designar(airePuro.Nombre, airePuro.Apariencia, "FuegoPuro", frecuenciaBaja);

            var aguaFuida = Designacion.Designar(aguaPura.Nombre, fuegoPuro.Apariencia, "Agua", frecuenciaAlta, 3 * Math.PI / 2);
            var tierraSolida = Designacion.Designar(tierraPura.Nombre, aguaFuida.Apariencia, "Tierra", frecuenciaMediaBaja);            
            var aireDisperso = Designacion.Designar(aguaPura.Nombre, tierraSolida.Apariencia, "Aire", frecuenciaMediaAlta);
            var fuegoCaliente = Designacion.Designar(fuegoPuro.Nombre, aireDisperso.Apariencia, "Fuego", frecuenciaBaja, 0);

            var yo = Designacion.Designar(fuegoCaliente.Nombre, fuegoCaliente.Apariencia, "Yo", frecuenciaExtrema, Math.PI);
            logger.LogInformation(yo.ToString());
            logger.LogInformation("═══ Ambiente configurado ═══\n");
            PilotosOfdmFrame = new List<Designacion> { tierraPura, aguaPura, airePuro, fuegoPuro };
            SubPilotosOfdmFrame = new List<Designacion> { aguaFuida, tierraSolida, aireDisperso, fuegoCaliente };
            return fuegoPuro;
        }
    }
}
