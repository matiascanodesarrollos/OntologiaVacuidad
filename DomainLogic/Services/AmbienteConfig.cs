using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DomainLogic.Services
{
    public static class AmbienteConfig
    {
        public static List<Designacion> PilotosOfdmFrame { get; private set; } = new List<Designacion>();
        public static List<Designacion> SubPilotosOfdmFrame { get; private set; } = new List<Designacion>();
        public static Designacion CrearAmbiente()
        {
            var frecuenciaBase = 1000000;
            var espacio = Designacion.Imaginar("Esencial", "Espacio", "Ser", 0, 0);
            var tiempo = Designacion.Imaginar("Estado", "Tiempo", "Estar", frecuenciaBase, Math.PI);
            var mente = Designacion.Designar(espacio.Nombre, tiempo.Apariencia, "Mente", frecuenciaBase);

            var elementoTierra = Designacion.Imaginar("Solida", "Tierra", "Permanecer", frecuenciaBase + 2, 0);
            var elementoAgua = Designacion.Imaginar("Liquida", "Agua", "Fluir", frecuenciaBase - 1, Math.PI / 2);
            var elementoAire = Designacion.Imaginar("Gaseoso", "Aire", "Mover", frecuenciaBase + 1, Math.PI);
            var elementoFuego = Designacion.Imaginar("Caliente", "Fuego", "Plasmar", frecuenciaBase - 2, 3 * Math.PI / 2);
        
            var apariencia = Designacion.Imaginar("Fría", "Apariencia", "Parecer", frecuenciaBase, Math.PI / 2);
            var espacioTiempo = Designacion.Designar(apariencia.Nombre, mente.Apariencia, "Espacio-Tiempo", frecuenciaBase);
            var liquido = Designacion.Designar(elementoAgua.Nombre, mente.Apariencia, "Líquido", frecuenciaBase/10000, Math.PI);
            var solido = Designacion.Designar(elementoTierra.Nombre, mente.Apariencia, "Sólido", frecuenciaBase/10000);
            var gas = Designacion.Designar(elementoAire.Nombre, mente.Apariencia, "Gas", frecuenciaBase/1000);
            var plasma = Designacion.Designar(elementoFuego.Nombre, mente.Apariencia, "Plasma", 1);

            PilotosOfdmFrame = new List<Designacion> { liquido, solido, gas, plasma };
            SubPilotosOfdmFrame = new List<Designacion> { mente, espacioTiempo, elementoTierra, elementoAgua, elementoAire, elementoFuego };
            return mente;
        }
    }
}
