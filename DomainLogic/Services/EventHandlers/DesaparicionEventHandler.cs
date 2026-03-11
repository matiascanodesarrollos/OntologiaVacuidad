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
        private static readonly Random _random = new Random();

        public DesaparicionEventHandler(IMediator mediator, ILogger<DesaparicionEventHandler> logger, ServiceConfig config)
        {
            _mediator = mediator;
            _logger = logger;
            _config = config;
        }

        public async Task Handle(DesaparicionEvent notification, CancellationToken cancellationToken)
        {
            double delay = _config.MinDelaySeconds + _random.NextDouble() * (_config.MaxDelaySeconds - _config.MinDelaySeconds);
            await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);
            var nombreOriginal = notification.NombreOrigen;
            var nuevaDesignacionProyectada = Designacion.Imaginar(nombreOriginal.Naturaleza.Texto, 
                nombreOriginal.Texto, 
                nombreOriginal.Causa.Texto,
                nombreOriginal.Causa.Frecuencia + _random.Next(2), // 50% de probabilidad de no encontrarlo
                nombreOriginal.Naturaleza.Fase);
            var nuevoNombre = Math.Abs(nuevaDesignacionProyectada.Frecuencia - nombreOriginal.Causa.Frecuencia) <= 1
                ? nombreOriginal
                : nuevaDesignacionProyectada.Nombre; 
            var nuevaDesignacion = Designacion.Designar(nuevoNombre, nombreOriginal.Efecto, nombreOriginal.Texto);
            await _mediator.Publish(new DesignacionEvent(nuevaDesignacion, null), cancellationToken);
            _logger.LogInformation($"[DESIGNACION-ENQUEUED] {nuevaDesignacion.Texto} (de Desaparición en {nombreOriginal.Texto})");
        }
    }
}
