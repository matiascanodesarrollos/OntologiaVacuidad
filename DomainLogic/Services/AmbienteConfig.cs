using Microsoft.Extensions.Logging;
using System;

namespace DomainLogic.Services
{
    public static class AmbienteConfig
    {
        public static Designacion CrearAmbiente(ILogger logger)
        {
            logger.LogInformation("═══ Configurando ambiente ═══\n");
            var espacio = Designacion.Crear("Vasto", "Espacio", "Estar", 0, 100000);
            var tiempo = Designacion.Crear("Puro", "Tiempo", "Ser", 2 * Math.PI, 0);
            var espacioTiempo = Designacion.Designar(espacio.Nombre, tiempo.Apariencia, "EspacioTiempo", 0, 100000);            
            var tierraSolida = Designacion.Designar(espacioTiempo.Nombre, espacioTiempo.Apariencia, "Tierra", Math.PI/2, 1000);
            var aguaFuida = Designacion.Designar(tierraSolida.Nombre, tierraSolida.Apariencia, "Agua", Math.PI/2, 400);
            var aireDisperso = Designacion.Designar(aguaFuida.Nombre, aguaFuida.Apariencia, "Aire", Math.PI, 600);
            var fuegoCaliente = Designacion.Designar(aireDisperso.Nombre, aireDisperso.Apariencia, "Fuego", 3 * Math.PI / 2, 200);
            var tierraPura = Designacion.Designar(fuegoCaliente.Nombre, fuegoCaliente.Apariencia, "TierraPura", 0, 400);
            var aguaPura = Designacion.Designar(tierraPura.Nombre, tierraPura.Apariencia, "AguaPura", Math.PI / 2, 1000);
            var aireComprimido = Designacion.Designar(aguaPura.Nombre, aguaPura.Apariencia, "AireComprimido", Math.PI, 600);
            var fuegoEstelar = Designacion.Designar(aireComprimido.Nombre, aireComprimido.Apariencia, "FuegoEstelar", 3 * Math.PI / 2, 200);
            var yo = Designacion.Designar(fuegoEstelar.Nombre, fuegoEstelar.Apariencia, "Yo", Math.PI, 10000);
            logger.LogInformation(yo.ToString());
            logger.LogInformation("═══ Ambiente configurado ═══\n");
            return fuegoEstelar;
        }
    }
}
