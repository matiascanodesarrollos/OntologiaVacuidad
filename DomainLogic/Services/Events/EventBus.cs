using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace DomainLogic.Services
{
    /// <summary>
    /// In-memory event bus for publishing domain events
    /// </summary>
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
            where TEvent : INotification;
    }

    /// <summary>
    /// MediatR-based implementation of the event bus
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly IMediator _mediator;

        public EventBus(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
            where TEvent : INotification
        {
            await _mediator.Publish(@event, cancellationToken);
        }
    }
}
