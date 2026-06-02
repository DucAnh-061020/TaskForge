using TaskForge.Domain.Events;

namespace TaskForge.Domain.Aggregates.Project.Events;

public record ProjectCreatedEvent(
    Guid ProjectId,
    string Name,
    string Description,
    Guid OwnerId) : DomainEvent(ProjectId);