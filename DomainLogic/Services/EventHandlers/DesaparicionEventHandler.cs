using MediatR;
using DomainLogic.Services.Events;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;

namespace DomainLogic.Services.EventHandlers
{
    /// <summary>
    /// Handler para el evento de desaparición de vibración
    /// Emite un DesignacionEvent para registrar el evento de desaparición
    /// </summary>
    public class DesaparicionEventHandler : INotificationHandler<DesaparicionEvent>        
    {        
        private readonly IMediator _mediator;
        private readonly ILogger<DesaparicionEventHandler> _logger;
        private readonly ServiceConfig _config;
        private readonly INombreSpaceRegistry _spaceRegistry;
        private static readonly Random _random = Random.Shared;

        public DesaparicionEventHandler(
            IMediator mediator, 
            ILogger<DesaparicionEventHandler> logger, 
            ServiceConfig config,
            INombreSpaceRegistry spaceRegistry)
        {
            _mediator = mediator;
            _logger = logger;
            _config = config;
            _spaceRegistry = spaceRegistry;
        }

        public async Task Handle(DesaparicionEvent notification, CancellationToken cancellationToken)
        {
            double delay = _config.MinDelaySeconds + _random.NextDouble() * (_config.MaxDelaySeconds - _config.MinDelaySeconds);
            await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);

            var nombreOriginal = notification.NombreOrigen;
            
            // Buscar el nombre más cercano en el espacio
            var nombreCercano = _spaceRegistry.FindNearestBySpaceProximity(nombreOriginal);
            
            if (nombreCercano != null)
            {
                // Usamos el nombre cercano encontrado
                _logger.LogInformation(
                    $"[DESAPARICION-BUSQUEDA] Buscando {nombreOriginal.Texto} en el espacio (Pos={nombreOriginal.Posicion:F3}, Dir={nombreOriginal.Direccion * (180 / Math.PI):F1}°) " +
                    $"→ Encontrado {nombreCercano.Texto} (Pos={nombreCercano.Posicion:F3}, Dir={nombreCercano.Direccion * (180 / Math.PI):F1}°)");
                
                var nuevaDesignacionAux = Designacion.Crear(
                    nombreCercano.Texto,
                    nombreCercano.Causa.Texto,
                    nombreCercano.Naturaleza.Texto,
                    nombreCercano.Causa.Frecuencia,
                    nombreCercano.Naturaleza.Fase);
                
                var resultado = nombreCercano.Mostrarse(nuevaDesignacionAux, $"Buscar {nombreOriginal.Texto}");
                var nuevaDesignacion = resultado.Esencia;
                
                await _mediator.Publish(new DesignacionEvent(nuevaDesignacion, null), cancellationToken);
                _logger.LogInformation($"[DESIGNACION-BUSQUEDA] {nuevaDesignacion.Texto} (Matriz: {nombreCercano.Texto})");
            }
        }
    }
}
