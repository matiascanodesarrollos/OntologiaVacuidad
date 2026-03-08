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
            var frecuenciaBase = Math.PI / 2 * 1000000; //Frecuencia base para el espacio, se asume que es la más alta para que las demás interactuen con ella.            
            var frecuenciaAlta = frecuenciaBase/10;
            var frecuenciaMedia = frecuenciaBase/100;
            var frecuenciaMediaBaja = frecuenciaBase/1000;
            var frecuenciaBaja = frecuenciaBase/10000;
                    
            var tiempo = Designacion.Imaginar("Parecido", "Tiempo", "Parecer", 0, Math.PI);
            var espacio = Designacion.Imaginar("Estado", "Espacio", "Estar", frecuenciaBase, 0);
            var yo = Designacion.Designar(espacio.Nombre, tiempo.Apariencia, "Yo", 1);

            var tierraPura = Designacion.Imaginar("Pura", "Tierra", "Permanecer", frecuenciaAlta, 0);
            var aguaPura = Designacion.Imaginar("Pura", "Agua", "Fluir", frecuenciaMediaBaja, Math.PI / 2);            
            var airePuro = Designacion.Imaginar("Puro", "Aire", "Mover", frecuenciaMedia, Math.PI);
            var fuegoPuro = Designacion.Imaginar("Caliente", "Fuego", "Ser", frecuenciaBaja, 3 * Math.PI / 2);
            
            var yoPuro = Designacion.Imaginar("Frío", "Yo", "Ser", frecuenciaBase, 3 * Math.PI / 2);
            var espacioTiempo = Designacion.Designar(yoPuro.Nombre, yo.Apariencia, "Espacio-Tiempo", 1);

            var agua = Designacion.Designar(aguaPura.Nombre, espacioTiempo.Apariencia, "Agua", 0.5, Math.PI);
            var tierra = Designacion.Designar(tierraPura.Nombre, agua.Apariencia, "Tierra", 0.5);
            var aire = Designacion.Designar(airePuro.Nombre, tierra.Apariencia, "Aire", 0.5);
            var fuego = Designacion.Designar(fuegoPuro.Nombre, aire.Apariencia, "Fuego", 0.03);

            logger.LogInformation(fuego.ToString());
            logger.LogInformation("═══ Ambiente configurado ═══\n");
            PilotosOfdmFrame = new List<Designacion> { agua, tierra, aire, fuego };
            SubPilotosOfdmFrame = new List<Designacion> { espacioTiempo, yoPuro, tierraPura, aguaPura, airePuro, fuegoPuro };
            return fuego;
        }
    }
}
