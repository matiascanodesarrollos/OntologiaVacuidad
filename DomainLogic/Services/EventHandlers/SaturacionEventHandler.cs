using MediatR;
using DomainLogic.Services.Events;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DomainLogic.Services.EventHandlers
{
    /// <summary>
    /// Handler para el evento de saturación de vibración
    /// Emite un DesignacionEvent para registrar el evento de saturación
    /// </summary>
    public class SaturacionEventHandler : INotificationHandler<SaturacionEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SaturacionEventHandler> _logger;
        private readonly ServiceConfig _config;

        public SaturacionEventHandler(IMediator mediator, ILogger<SaturacionEventHandler> logger, ServiceConfig config)
        {
            _mediator = mediator;
            _logger = logger;
            _config = config;
        }

        public async Task Handle(SaturacionEvent notification, CancellationToken cancellationToken)
        {
            // Emitir evento de designación con delay aleatorio
            double delay = _config.MinDelaySeconds + new Random().NextDouble() * (_config.MaxDelaySeconds - _config.MinDelaySeconds);
            await Task.Delay(TimeSpan.FromSeconds(delay));
            var nombre = notification.NombreOrigen;
            var nuevaDesignacion = Designacion.Designar(nombre, nombre.Causa.Apariencia, nombre.Texto);
            var designacionEvent = new DesignacionEvent(nuevaDesignacion);
            lock (ServiceConfig.LogLock)
            {
                _logger.LogInformation($"Nueva designacion por saturación: {notification.NombreOrigen.Texto}.");
            }            
            await _mediator.Publish(designacionEvent, cancellationToken);
        }
    }
}
