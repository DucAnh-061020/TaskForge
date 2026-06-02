using TaskForge.Domain.Events;

namespace TaskForge.Domain.Aggregates.BoardTask.Events;

public record TaskMovedEvent(
    Guid TaskId,
    Guid OldColumnId,
    Guid NewColumnId) : DomainEvent(TaskId);