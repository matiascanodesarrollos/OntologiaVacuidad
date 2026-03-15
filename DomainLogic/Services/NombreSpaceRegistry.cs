using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainLogic.Services
{
    /// <summary>
    /// Mantiene un registro de nombres en el espacio para permitir búsquedas de proximidad basadas en propiedades vectoriales
    /// </summary>
    public interface INombreSpaceRegistry
    {
        void Register(Nombre nombre);
        void Unregister(Guid nombreId);
        Nombre? FindNearestBySpaceProximity(Nombre referencia, double posicionTolerance = 0.2, double direccionToleranceRad = Math.PI / 6);
        IEnumerable<Nombre> FindNearby(Nombre referencia, double proximityRadius = 0.3);
    }

    public class NombreSpaceRegistry : INombreSpaceRegistry
    {
        private readonly object _sync = new object();
        private readonly Dictionary<Guid, Nombre> _nombres = new();

        public void Register(Nombre nombre)
        {
            lock (_sync)
            {
                _nombres[nombre.Id] = nombre;
            }
        }

        public void Unregister(Guid nombreId)
        {
            lock (_sync)
            {
                _nombres.Remove(nombreId);
            }
        }

        public Nombre? FindNearestBySpaceProximity(Nombre referencia, double posicionTolerance = 0.2, double direccionToleranceRad = Math.PI / 6)
        {
            lock (_sync)
            {
                var candidatos = _nombres.Values
                    .Where(n => n.Id != referencia.Id)
                    .Where(n => IsSpaceProximate(referencia, n, posicionTolerance, direccionToleranceRad))
                    .ToList();

                if (candidatos.Count == 0)
                    return null;

                // Retornar el más cercano por distancia espacial
                return candidatos.OrderBy(n => CalculateSpaceDistance(referencia, n)).First();
            }
        }

        public IEnumerable<Nombre> FindNearby(Nombre referencia, double proximityRadius = 0.3)
        {
            lock (_sync)
            {
                return _nombres.Values
                    .Where(n => n.Id != referencia.Id)
                    .Where(n => CalculateSpaceDistance(referencia, n) <= proximityRadius)
                    .OrderBy(n => CalculateSpaceDistance(referencia, n))
                    .ToList();
            }
        }

        private bool IsSpaceProximate(Nombre ref1, Nombre ref2, double posicionTolerance, double direccionToleranceRad)
        {
            double posicionDiff = Math.Abs(ref1.Posicion - ref2.Posicion);
            
            // Normalizar diferencia de dirección al rango [0, π]
            double direccionDiff = Math.Abs(ref1.Direccion - ref2.Direccion);
            if (direccionDiff > Math.PI)
                direccionDiff = 2 * Math.PI - direccionDiff;

            // Debe estar dentro de tolerancia en ambas dimensiones
            return posicionDiff <= posicionTolerance && 
                   (direccionDiff <= direccionToleranceRad || Math.Abs(direccionDiff - Math.PI) <= direccionToleranceRad);
        }

        private double CalculateSpaceDistance(Nombre ref1, Nombre ref2)
        {
            // Distancia euclidiana en el espacio de (posición, dirección, velocidad)
            // Normalizar dirección para la distancia
            double direccionDiff = Math.Abs(ref1.Direccion - ref2.Direccion);
            if (direccionDiff > Math.PI)
                direccionDiff = 2 * Math.PI - direccionDiff;

            double posicionDiff = Math.Abs(ref1.Posicion - ref2.Posicion);
            double velocidadDiff = Math.Abs(ref1.Velocidad - ref2.Velocidad);

            // Ponderación: posición y dirección más importantes que velocidad
            double distancia = Math.Sqrt(
                Math.Pow(posicionDiff, 2) * 0.5 +
                Math.Pow(direccionDiff / Math.PI, 2) * 0.3 +
                Math.Pow(velocidadDiff, 2) * 0.2
            );

            return distancia;
        }
    }
}
