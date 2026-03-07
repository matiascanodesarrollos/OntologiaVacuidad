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
    /// Handler para el evento de desaparición de vibración
    /// Emite un DesignacionEvent para registrar el evento de desaparición
    /// </summary>
    public class DesaparicionEventHandler : INotificationHandler<DesaparicionEvent>        
    {        
        private readonly IMediator _mediator;
        private readonly ILogger<DesaparicionEventHandler> _logger;
        private readonly ServiceConfig _config;

        public DesaparicionEventHandler(IMediator mediator, ILogger<DesaparicionEventHandler> logger, ServiceConfig config)
        {
            _mediator = mediator;
            _logger = logger;
            _config = config;
        }

        public async Task Handle(DesaparicionEvent notification, CancellationToken cancellationToken)
        {
            // Emitir evento de designación            
            double delay = _config.MinDelaySeconds + new Random().NextDouble() * (_config.MaxDelaySeconds - _config.MinDelaySeconds);
            await Task.Delay(TimeSpan.FromSeconds(delay));
            var nombre = notification.NombreOrigen;
            var apariencia = nombre.BuscarSignificado().Last();
            var nuevaDesignacion = Designacion.Designar(nombre, apariencia, nombre.Texto);
            var designacionEvent = new DesignacionEvent(nuevaDesignacion);
            lock (ServiceConfig.LogLock)
            {
                _logger.LogInformation($"Nueva designacion por desaparición de {notification.NombreOrigen.Texto}.");                
            }
            await _mediator.Publish(designacionEvent, cancellationToken);            
        }
    }
}
