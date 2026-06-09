using TaskForge.Domain.Events;

namespace TaskForge.Domain.Aggregates.Board.Events;

public record ColumnAddedEvent(Guid BoardId, Guid ColumnId, string Name, int Order) : DomainEvent(BoardId);