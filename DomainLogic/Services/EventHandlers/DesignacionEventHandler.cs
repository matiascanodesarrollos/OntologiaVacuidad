using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

public class DesignacionEventHandler : INotificationHandler<DesignacionEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<DesignacionEventHandler> _logger;

    public DesignacionEventHandler(IMediator mediator, ILogger<DesignacionEventHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(DesignacionEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(notification.NuevaDesignacion.ToString());
        await notification.NuevaDesignacion.Nombre.Vibrar(_mediator, _logger);
    }
}
