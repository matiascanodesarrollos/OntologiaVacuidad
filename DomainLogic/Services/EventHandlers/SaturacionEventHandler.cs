using MediatR;
using DomainLogic.Services.Events;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;

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
        private readonly Random _random = new Random();

        public SaturacionEventHandler(IMediator mediator, ILogger<SaturacionEventHandler> logger, ServiceConfig config)
        {
            _mediator = mediator;
            _logger = logger;
            _config = config;
        }

        public async Task Handle(SaturacionEvent notification, CancellationToken cancellationToken)
        {
            // Emitir evento de designación            
            double delay = _config.MinDelaySeconds + _random.NextDouble() * (_config.MaxDelaySeconds - _config.MinDelaySeconds);
            await Task.Delay(TimeSpan.FromSeconds(delay));
            var nombreOriginal = notification.NombreOrigen;
            var nuevaDesignacion = Designacion.Designar(nombreOriginal, nombreOriginal.Efecto, nombreOriginal.Texto);
            var designacionEvent = new DesignacionEvent(nuevaDesignacion, notification.Stack);
            await _mediator.Publish(designacionEvent, cancellationToken);
        }
    }
}
