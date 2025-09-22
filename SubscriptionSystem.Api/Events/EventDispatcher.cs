namespace SubscriptionSystem.Events;

public interface IEventDispatcher
{
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent;
}

public class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public EventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
    {
        using var scope = _serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>();

        foreach (var handler in handlers)
        {
            await handler.HandleAsync(@event);
        }
    }
}
