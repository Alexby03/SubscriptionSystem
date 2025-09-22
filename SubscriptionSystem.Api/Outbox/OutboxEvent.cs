namespace SubscriptionSystem.Outbox;

public class OutboxEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EventType { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool Processed { get; set; } = false;
}
