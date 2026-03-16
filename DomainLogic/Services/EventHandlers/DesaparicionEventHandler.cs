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
        private static readonly Random _random = Random.Shared;

        public DesaparicionEventHandler(
            IMediator mediator, 
            ILogger<DesaparicionEventHandler> logger, 
            ServiceConfig config)
        {
            _mediator = mediator;
            _logger = logger;
            _config = config;
        }

        public async Task Handle(DesaparicionEvent notification, CancellationToken cancellationToken)
        {
            double delay = _config.MinDelaySeconds + _random.NextDouble() * (_config.MaxDelaySeconds - _config.MinDelaySeconds);
            await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);
            
            await _mediator.Publish(new DesignacionEvent(notification.NombreOrigen.Causa, null), cancellationToken);
                _logger.LogInformation($"[DESIGNACION-BUSQUEDA] {notification.NombreOrigen.Causa.Texto} (Matriz: {notification.NombreOrigen.Texto})");
        }
    }
}
