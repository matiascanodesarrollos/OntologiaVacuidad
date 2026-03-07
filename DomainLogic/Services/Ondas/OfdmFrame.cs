using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainLogic.Services.Ondas
{
    /// <summary>
    /// Simula una estructura OFDM mejorada con distribución inteligente de datos
    /// Combina: simetría radial + agrupación por naturaleza + alternancia de espacios
    /// </summary>
    public static class OfdmFrame
    {
        public static List<OscillatorSignal> CrearOfdmFrame(this Designacion designacion)
        {
            // Estructura mejorada: combina simetría radial, agrupación por naturaleza y alternancia
            // Centro: datos principales (mayor amplitud)
            // Difusión: espaciados y agrupados por naturaleza
            
            var frame = Enumerable.Range(0, 64).Select(i => new OscillatorSignal(0, 0)).ToList();
            
            // Posiciones fijas
            var vacios = new HashSet<int> { 0, 1, 2, 3, 4, 5, 32, 59, 60, 61, 62, 63 };
            var pilotosIndices = new List<int> { 11, 25, 39, 53 };
            
            // Obtener datos y agrupar por naturaleza (Causa.Texto)
            var datos = designacion.Nombre.BuscarSignificado(48); // Limitar a 48 datos para evitar sobrecargar el frame
            var gruposPorNaturaleza = datos
                .GroupBy(d => d.Causa.Texto)
                .OrderByDescending(g => g.Average(d => d.Amplitud))  // Mayor energía al centro
                .ToList();
            
            // Estrategia de distribución: simetría radial desde el centro hacia afuera
            int centerPos = 32;
            var posicionesOrdenadas = new List<int>();
            
            // Construir lista de posiciones desde el centro hacia afuera
            for (int dist = 1; dist <= 24; dist++)
            {
                // Izquierda del centro
                if (centerPos - dist >= 0 && !vacios.Contains(centerPos - dist) && !pilotosIndices.Contains(centerPos - dist))
                    posicionesOrdenadas.Add(centerPos - dist);
                // Derecha del centro
                if (centerPos + dist < 64 && !vacios.Contains(centerPos + dist) && !pilotosIndices.Contains(centerPos + dist))
                    posicionesOrdenadas.Add(centerPos + dist);
            }
            
            // Colocar pilotos
            int pilotoIndex = 0;
            foreach (int pilotoPos in pilotosIndices)
            {
                var pilotoDesignacion = AmbienteConfig.PilotosOfdmFrame[pilotoIndex++ % AmbienteConfig.PilotosOfdmFrame.Count];
                frame[pilotoPos] = new OscillatorSignal(
                    pilotoDesignacion.Frecuencia,
                    pilotoDesignacion.Apariencia.Amplitud,
                    pilotoDesignacion.Nombre.Naturaleza.Fase);
            }
            
            // Distribuir datos: alternar grupos naturaleza, dejar espacios para desacoplamiento
            int posIndex = 0;
            int datosEnGrupo = 0;
            int maxDatosPerGrupo = Math.Max(1, posicionesOrdenadas.Count / (gruposPorNaturaleza.Count * 2));
            
            foreach (var grupo in gruposPorNaturaleza)
            {
                foreach (var dato in grupo)
                {
                    if (posIndex >= posicionesOrdenadas.Count)
                        break;
                    
                    int pos = posicionesOrdenadas[posIndex];
                    frame[pos] = new OscillatorSignal(
                        dato.Esencia.Frecuencia,
                        dato.Amplitud,
                        dato.Causa.Naturaleza.Fase);
                    
                    datosEnGrupo++;
                    
                    // Saltar posiciones para crear espacios entre agrupaciones (alternancia)
                    if (datosEnGrupo >= maxDatosPerGrupo)
                    {
                        posIndex += 2;  // Dejar espacio
                        datosEnGrupo = 0;
                    }
                    else
                    {
                        posIndex++;
                    }
                }
            }
            
            // Llenar posiciones restantes si hay más datos
            while (posIndex < posicionesOrdenadas.Count && datos.Count > 0)
            {
                for (int i = 0; i < datos.Count && posIndex < posicionesOrdenadas.Count; i++)
                {
                    int pos = posicionesOrdenadas[posIndex];
                    frame[pos] = new OscillatorSignal(
                        datos[i].Esencia.Frecuencia,
                        datos[i].Amplitud,
                        datos[i].Causa.Naturaleza.Fase);
                    posIndex += 2;  // Mantener espaciado
                }
            }
            
            // Agregar subpilotos en posiciones estándar si hay espacio disponible
            // Subpilotos en primeros y últimos 2 canales de datos (si no fueron ocupados)
            if (datos.Count < 48)
            {
                var subPilotosIndices = new List<int> { 6, 7, 57, 58 };
                for (int i = 0; i < subPilotosIndices.Count; i++)
                {
                    int pos = subPilotosIndices[i];
                    // Solo colocar subpiloto si la posición está vacía
                    if (frame[pos].Amplitude == 0)
                    {
                        var subPilotoDesignacion = AmbienteConfig.SubPilotosOfdmFrame[i % AmbienteConfig.SubPilotosOfdmFrame.Count];
                        frame[pos] = new OscillatorSignal(
                            subPilotoDesignacion.Frecuencia,
                            subPilotoDesignacion.Apariencia.Amplitud,
                            subPilotoDesignacion.Nombre.Naturaleza.Fase);
                    }
                }
            }
            
            return frame;
        }
    }
}
