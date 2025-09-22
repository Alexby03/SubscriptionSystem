using Microsoft.EntityFrameworkCore;
using SubscriptionSystem.Data;
using SubscriptionSystem.Events;
using SubscriptionSystem.Services;
using System.Text.Json;

namespace SubscriptionSystem.Outbox;

public class OutboxWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxWorker> _logger;

    public OutboxWorker(IServiceProvider serviceProvider, ILogger<OutboxWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var dispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();

                var events = await db.OutboxEvents
                    .Where(e => !e.Processed)
                    .OrderBy(e => e.CreatedAt)
                    .Take(20)
                    .ToListAsync(stoppingToken);

                foreach (var outbox in events)
                {
                    var type = Type.GetType($"SubscriptionSystem.Events.{outbox.EventType}");
                    if (type == null) continue;
                    var @event = JsonSerializer.Deserialize(outbox.Payload, type);
                    if (@event == null) continue;

                    await dispatcher.PublishAsync((dynamic)@event);
                    outbox.Processed = true;
                    _logger.LogInformation("Outbox Event processed: {EventId} ({EventType})", outbox.Id, outbox.EventType);
                }
                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox events");
            }

            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }

        _logger.LogInformation("Outbox worker stopped.");
    }
}
