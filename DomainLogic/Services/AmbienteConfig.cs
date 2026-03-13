using System;

namespace DomainLogic.Services
{
    public static class AmbienteConfig
    {
        public static Designacion CrearAmbiente()
        {
            var frecuenciaBase = 1000000;
            var espacio = Designacion.Crear("Espacio", "Ser", "Esencial", 0, 0);
            var tiempo = Designacion.Crear("Tiempo", "Estar", "Estado", frecuenciaBase, Math.PI / 2);
            var cuerpo = Designacion.Crear("Cuerpo", "Parecer", "Aparente", 0, Math.PI);
            var mente = Designacion.Crear("Mente", "Aclarar", "Claridad", frecuenciaBase, 3 * Math.PI / 2);
            var materia = cuerpo.Nombre.Mostrarse(espacio);
            var antiMateria = mente.Nombre.Mostrarse(tiempo);

            var elementoTierra = Designacion.Crear("Tierra", "Permanecer", "Sólida", frecuenciaBase + 2, 0);
            var elementoAgua = Designacion.Crear("Agua", "Fluir", "Líquida", frecuenciaBase - 1, Math.PI / 2);
            var elementoAire = Designacion.Crear("Aire", "Mover", "Gaseoso", frecuenciaBase + 1, Math.PI);
            var elementoFuego = Designacion.Crear("Fuego", "Calentar", "Plasma", frecuenciaBase - 2, 3 * Math.PI / 2);
        
            var apariencia = antiMateria.Esencia.Nombre.Mostrarse(materia.Esencia, 0, 0);
            apariencia.Esencia.Nombre.Mostrarse(elementoAgua, 500, 0); //Estado líquido
            apariencia.Esencia.Nombre.Mostrarse(elementoTierra, 500, Math.PI / 2); //Estado sólido
            apariencia.Esencia.Nombre.Mostrarse(elementoAire, 600, Math.PI); //Estado gaseoso
            apariencia.Esencia.Nombre.Mostrarse(elementoFuego, 100, 3 * Math.PI / 2); //Estado plasma
            return apariencia.Esencia;
        }
    }
}
