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
            var frecuenciaBase = 1000000;
            var frecuenciaAlta = frecuenciaBase/10;
            var frecuenciaMedia = frecuenciaBase/100;
            var frecuenciaMediaBaja = frecuenciaBase/1000;
            var frecuenciaBaja = frecuenciaBase/10000;
                       
            var espacio = Designacion.Imaginar("Estado", "Espacio", "Estar", frecuenciaBase, 0);
            var tiempo = Designacion.Imaginar("Parecido", "Tiempo", "Parecer", 0, Math.PI);
            var mente = Designacion.Designar(espacio.Nombre, tiempo.Apariencia, "Mente", 1);

            var elementoTierra = Designacion.Imaginar("Solida", "Tierra", "Permanecer", frecuenciaAlta, 0);
            var elementoAgua = Designacion.Imaginar("Liquida", "Agua", "Fluir", frecuenciaMediaBaja, Math.PI / 2);
            var elementoAire = Designacion.Imaginar("Gaseoso", "Aire", "Mover", frecuenciaMedia, Math.PI);
            var elementoFuego = Designacion.Imaginar("Caliente", "Fuego", "Plasmar", frecuenciaBaja, 3 * Math.PI / 2);
        
            var yo = Designacion.Imaginar("Frío", "Yo", "Ser", frecuenciaBase, Math.PI / 2);
            var espacioTiempo = Designacion.Designar(yo.Nombre, mente.Apariencia, "Espacio-Tiempo", 1);
            var liquido = Designacion.Designar(elementoAgua.Nombre, espacioTiempo.Apariencia, "Líquido", 3, Math.PI);
            var solido = Designacion.Designar(elementoTierra.Nombre, liquido.Apariencia, "Sólido", 0.5);
            var gas = Designacion.Designar(elementoAire.Nombre, solido.Apariencia, "Gas", 0.5);
            var plasma = Designacion.Designar(elementoFuego.Nombre, gas.Apariencia, "Plasma", 0.03);

            logger.LogInformation(plasma.ToString());
            logger.LogInformation("═══ Ambiente configurado ═══\n");
            PilotosOfdmFrame = new List<Designacion> { liquido, solido, gas, plasma };
            SubPilotosOfdmFrame = new List<Designacion> { mente, elementoTierra, elementoAgua, elementoAire, elementoFuego };
            return plasma;
        }
    }
}
