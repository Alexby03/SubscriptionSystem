using Azure.Messaging.ServiceBus;
using SubscriptionSystem.Events;

namespace SubscriptionSystem.Services;

public class AzureServiceBusDispatcher : IEventDispatcher
{
    private readonly ServiceBusClient _client;
    private readonly string _topicName;

    public AzureServiceBusDispatcher(string connectionString, string topicName)
    {
        _client = new ServiceBusClient(connectionString);
        _topicName = topicName;
    }

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
    {
        var sender = _client.CreateSender(_topicName);
        var json = System.Text.Json.JsonSerializer.Serialize(@event);
        var message = new ServiceBusMessage(json)
        {
            Subject = typeof(TEvent).Name //metadata containing event type
        };
        message.ApplicationProperties["EventType"] = typeof(TEvent).Name;
        await sender.SendMessageAsync(message);
    }
}
