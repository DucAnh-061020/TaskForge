namespace TaskForge.Infrastructure.Data.Entities;

public class EventEntity
{
    public Guid StreamId { get; set; }
    public int Version { get; set; }
    public string EventType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Data { get; set; } = "{}";
    public string Metadata { get; set; } = "{}";
}