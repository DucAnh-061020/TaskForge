using System;
using TaskForge.Domain.Events;

namespace TaskForge.Domain.Aggregates.BoardTask.Events;

public record TaskCreatedEvent(
    Guid TaskId,
    string Title,
    string Description,
    Guid ColumnId,
    Guid AssignedUserId) : DomainEvent(TaskId);
