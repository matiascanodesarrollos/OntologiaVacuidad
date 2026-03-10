using System.Threading;
using System.Threading.Tasks;
using DomainLogic.Services;
using MediatR;
using Microsoft.Extensions.Logging;

public class DesignacionEventHandler : INotificationHandler<DesignacionEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<DesignacionEventHandler> _logger;
    private readonly IDesignacionQueue _designacionQueue;
    public DesignacionEventHandler(IMediator mediator, ILogger<DesignacionEventHandler> logger, IDesignacionQueue designacionQueue)
    {
        _mediator = mediator;
        _logger = logger;
        _designacionQueue = designacionQueue;
    }

    public async Task Handle(DesignacionEvent notification, CancellationToken cancellationToken)
    {
        _designacionQueue.Enqueue(notification.NuevaDesignacion);
        await Task.CompletedTask;
    }
}
