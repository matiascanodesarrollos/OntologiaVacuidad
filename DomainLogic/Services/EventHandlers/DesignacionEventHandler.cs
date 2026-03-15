using System.Threading;
using System.Threading.Tasks;
using DomainLogic.Services;
using MediatR;

public class DesignacionEventHandler : INotificationHandler<DesignacionEvent>
{
    private readonly IDesignacionQueue _designacionQueue;
    private readonly INombreSpaceRegistry _spaceRegistry;
    
    public DesignacionEventHandler(IDesignacionQueue designacionQueue, INombreSpaceRegistry spaceRegistry)
    {
        _designacionQueue = designacionQueue;
        _spaceRegistry = spaceRegistry;
    }

    public Task Handle(DesignacionEvent notification, CancellationToken cancellationToken)
    {
        _designacionQueue.Enqueue(notification.NuevaDesignacion);
        
        // Registrar el nombre de la nueva designación en el espacio
        _spaceRegistry.Register(notification.NuevaDesignacion.Nombre);
        
        return Task.CompletedTask;
    }
}
