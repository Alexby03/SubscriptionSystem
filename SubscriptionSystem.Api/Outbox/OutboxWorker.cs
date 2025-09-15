using Microsoft.EntityFrameworkCore;
using SubscriptionSystem.Data;
using SubscriptionSystem.Events;
using System.Text.Json;

namespace SubscriptionSystem.Workers;

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
                    .Take(20) // batch size
                    .ToListAsync(stoppingToken);

                foreach (var outbox in events)
                {
                    var type = Type.GetType($"SubscriptionSystem.Events.{outbox.EventType}");
                    if (type == null) continue;

                    var @event = JsonSerializer.Deserialize(outbox.Payload, type);
                    if (@event == null) continue;

                    var method = typeof(IEventDispatcher).GetMethod("PublishAsync")?.MakeGenericMethod(type);

                    if (method != null)
                    {
                        var task = (Task)method.Invoke(dispatcher, new[] { @event })!;
                        if (task != null)
                        {
                            await task;
                            outbox.Processed = true;
                        }
                    }
                }

                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox events");
            }

            await Task.Delay(TimeSpan.FromSeconds(300), stoppingToken); // waittime next scan
        }

        _logger.LogInformation("Outbox worker stopped.");
    }
}
