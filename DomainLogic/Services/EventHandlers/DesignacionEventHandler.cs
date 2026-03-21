using System.Threading;
using System.Threading.Tasks;
using MediatR;

public class DesignacionEventHandler : INotificationHandler<DesignacionEvent>
{    
    public DesignacionEventHandler()
    {
    }

    public Task Handle(DesignacionEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
