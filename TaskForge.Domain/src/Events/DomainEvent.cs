using MediatR;

namespace TaskForge.Domain.Events;

public interface IDomainEvent : INotification
{
    Guid Id { get; }
    Guid AggregateId { get; }
    int Version { get; }
    DateTime OccurredOn { get; }
}

public abstract record DomainEvent(Guid AggregateId) : IDomainEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public int Version { get; internal set; } // Managed by the AggregateRoot during lifecycle
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
