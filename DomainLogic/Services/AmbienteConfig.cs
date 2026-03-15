using System;

namespace DomainLogic.Services
{
    public static class AmbienteConfig
    {
        public static Designacion CrearAmbiente()
        {
            var frecuenciaMaxima = 1000000;
            var espacio = Designacion.Crear("Espacio", "Ser", "Ente", frecuenciaMaxima, 0);
            var tiempo = Designacion.Crear("Tiempo", "Estar", "Estado", frecuenciaMaxima, Math.PI / 2);
            var espacioTiempo = tiempo.Nombre.Mostrarse(espacio, "Parecer espacio-tiempo");
            var yo = Designacion.Crear("Yo", "Aparecer", "Aparente", 0, Math.PI);
            var apariencia = yo.Nombre.Mostrarse(espacioTiempo.Esencia, "Ser Apariencia");

            var elementoTierra = Designacion.Crear("Tierra", "Permanecer", "Sólida", 1000, Math.PI / 2);
            var estadoSolido = elementoTierra.Nombre.Mostrarse(apariencia.Esencia, "Permanece líquida");
            
            var elementoAire = Designacion.Crear("Aire", "Mover", "Gaseoso", 600, Math.PI);
            var estadoGaseoso = elementoAire.Nombre.Mostrarse(estadoSolido.Esencia, "Mueve gas"); 

            var elementoFuego = Designacion.Crear("Fuego", "Calentar", "Plasma", 100, 3 * Math.PI / 2);
            var estadoPlasma = elementoFuego.Nombre.Mostrarse(estadoGaseoso.Esencia, "Calienta plasma");                        
            
            var elementoAgua = Designacion.Crear("Agua", "Fluir", "Líquida", 500, 0);
            var estadoLiquido = elementoAgua.Nombre.Mostrarse(estadoPlasma.Esencia, "Fluye sólida");

            return estadoLiquido.Esencia;
        }
    }
}
