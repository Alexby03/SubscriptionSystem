using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SubscriptionSystem.Events;
using System.Text.Json;

namespace SubscriptionSystem.Services;

public class AzureServiceBusWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AzureServiceBusWorker> _logger;
    private readonly ServiceBusClient _client;
    private readonly string _topicName;
    private readonly string _subscriptionName;
    private ServiceBusProcessor? _processor;

    public AzureServiceBusWorker(IServiceProvider serviceProvider, ILogger<AzureServiceBusWorker> logger, ServiceBusClient client, string topicName, string subscriptionName)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _client = client;
        _topicName = topicName;
        _subscriptionName = subscriptionName;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor = _client.CreateProcessor(_topicName, _subscriptionName, new ServiceBusProcessorOptions());

        _processor.ProcessMessageAsync += async args =>
        {
            var json = args.Message.Body.ToString();
            var typeName = args.Message.Subject;
            var type = Type.GetType($"SubscriptionSystem.Events.{args.Message.Subject}");

            if (type != null)
            {
                var @event = JsonSerializer.Deserialize(json, type);

                if (@event is IEvent evt)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var handlers = scope.ServiceProvider.GetServices(typeof(IEventHandler<>).MakeGenericType(type));
                    if(handlers.Any()) _logger.LogInformation("Azure Service Bus: Handling: {EventType}", typeName);
                    foreach (dynamic handler in handlers!)
                    {
                        await handler.HandleAsync((dynamic)evt);
                    }
                }
            }

            await args.CompleteMessageAsync(args.Message); //mark processed in Service Bus
        };

        _processor.ProcessErrorAsync += args =>
        {
            _logger.LogError(args.Exception, "Error in Service Bus processing");
            return Task.CompletedTask;
        };

        await _processor.StartProcessingAsync(stoppingToken);

        //-1 == keep running
        await Task.Delay(-1, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_processor != null)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}
