using System;

namespace DomainLogic.Services
{
    public static class AmbienteConfig
    {
        public static Apariencia CrearAmbiente()
        {
            var frecuenciaMaxima = 10000;
            var mente = Designacion.Crear("Mente", "Ser", "Luminosa", frecuenciaMaxima, 0);
            var cuerpo = Designacion.Crear("Cuerpo", "Parecer", "Sonoro", frecuenciaMaxima, Math.PI / 2);            
            var espacioTiempo = cuerpo.Esencia.Mostrarse(mente, "Está sólido");
            var yo = Designacion.Crear("Yo", "Estar", "Unido", 0, Math.PI);
            var apariencia = yo.Esencia.Mostrarse(espacioTiempo.NaturalezaAparente.Causa, "Mueve materia");

            var elementoTierra = Designacion.Crear("Tierra", "Permanecer", "Sólida", 1000, Math.PI / 2);
            var estadoSolido = elementoTierra.Esencia.Mostrarse(apariencia.NaturalezaAparente.Causa, "Permanece");
            
            var elementoAire = Designacion.Crear("Aire", "Mover", "Gaseoso", 600, Math.PI);
            var estadoGaseoso = elementoAire.Esencia.Mostrarse(estadoSolido.NaturalezaAparente.Causa, "Es materia");

            var elementoFuego = Designacion.Crear("Fuego", "Calentar", "Eléctrico", 100, 3 * Math.PI / 2);
            var estadoPlasma = elementoFuego.Esencia.Mostrarse(estadoGaseoso.NaturalezaAparente.Causa, "Calienta");
              
            var elementoAgua = Designacion.Crear("Agua", "Fluir", "Líquida", 500, 0);
            var estadoLiquido = elementoAgua.Esencia.Mostrarse(estadoPlasma.NaturalezaAparente.Causa, "Fluye materia");
            
            return estadoLiquido;
        }
    }
}
