using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainLogic.Services.Ondas
{
    /// <summary>
    /// Simula una estructura OFDM tipo 802.11a con DC, pilotos y canales de datos
    /// </summary>
    public static class OfdmFrame
    {
        public static List<OscillatorSignal> CrearOfdmFrame(this Designacion designacion, List<Designacion> pilotos)
        {
            // Estructura estándar 802.11a: 1 DC, 4 pilotos, 48 canales de datos, 11 vacíos
            // Ejemplo: [vacío, datos(6), piloto, datos(5), vacío, datos(5), piloto, datos(5), vacío, datos(5), piloto, datos(5), vacío, datos(5), piloto, datos(6), vacío]            
            int dataIndex = 0;
            int pilotoIndex = 0;
            int totalSubcarriers = 64;
            var frame = Enumerable.Range(0, 64).Select(i => new OscillatorSignal(0, 0)).ToList();
            var datos = designacion.Nombre.BuscarSignificado();
            for (int i = 0; i < totalSubcarriers; i++)
            {
                if (i == 11 || i == 25 || i == 39 || i == 53)
                {
                    var pilotoDesignacion = pilotos[pilotoIndex++ % pilotos.Count];
                    frame.Add(new OscillatorSignal(
                        pilotoDesignacion.Frecuencia, 
                        pilotoDesignacion.Apariencia.Amplitud, 
                        pilotoDesignacion.Nombre.Naturaleza.Fase));
                    continue;
                }
                var nombreData = datos[dataIndex++ % datos.Count];
                frame.Add(new OscillatorSignal(
                    nombreData.Esencia.Frecuencia, 
                    nombreData.Amplitud, 
                    nombreData.Causa.Naturaleza.Fase));
            }

            return frame;
        }
    }
}
