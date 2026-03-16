using System.Threading;
using System.Threading.Tasks;
using DomainLogic.Services;
using MediatR;

public class DesignacionEventHandler : INotificationHandler<DesignacionEvent>
{
    private readonly IDesignacionQueue _designacionQueue;
    
    public DesignacionEventHandler(IDesignacionQueue designacionQueue)
    {
        _designacionQueue = designacionQueue;
    }

    public Task Handle(DesignacionEvent notification, CancellationToken cancellationToken)
    {
        _designacionQueue.Enqueue(notification.NuevaDesignacion);        
        return Task.CompletedTask;
    }
}
