using TaskForge.Domain.Events;

namespace TaskForge.Domain.Aggregates.Board.Events;

public record BoardCreatedEvent(Guid BoardId, string Name, Guid ProjectId) : DomainEvent(BoardId);