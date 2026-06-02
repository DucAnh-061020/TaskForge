using TaskForge.Domain.Aggregates.Project.Events;
using TaskForge.Domain.Events;
using TaskForge.Domain.Primitives;

namespace TaskForge.Domain.Aggregates.Project;

public class ProjectAggregate : AggregateRoot
{
    // Aggregate State Properties
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid OwnerId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Internal track listing of Board IDs belonging to this project context
    private readonly List<Guid> _boardIds = new();
    public IReadOnlyCollection<Guid> BoardIds => _boardIds.AsReadOnly();

    // Required parameterless constructor for historical stream rehydration
    private ProjectAggregate() { }

    /// <summary>
    /// Factory pattern to instantiate a valid, rule-compliant Project stream.
    /// </summary>
    public static ProjectAggregate Create(Guid projectId, string name, string description, Guid ownerId)
    {
        // Domain Guard Clauses & Invariants
        if (projectId == Guid.Empty)
            throw new ArgumentException("Project Identifier cannot be empty.", nameof(projectId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name cannot be empty or whitespace.", nameof(name));

        if (ownerId == Guid.Empty)
            throw new ArgumentException("Project must be assigned to a valid owner.", nameof(ownerId));

        var project = new ProjectAggregate();

        // Raise the creation event up to the append log queue
        project.RaiseEvent(new ProjectCreatedEvent(
            projectId,
            name.Trim(),
            description?.Trim() ?? string.Empty,
            ownerId
        ));

        return project;
    }

    /// <summary>
    /// Self-contained rehydration routine matching our pattern signature
    /// </summary>
    public static ProjectAggregate Rehydrate(IEnumerable<DomainEvent> history)
    {
        var project = new ProjectAggregate();
        project.LoadFromHistory(history);
        return project;
    }

    // =========================================================================
    // State Routing - Strongly Typed Internal Transitions
    // =========================================================================

    private void When(ProjectCreatedEvent @event)
    {
        Id = @event.AggregateId;
        Name = @event.Name;
        Description = @event.Description;
        OwnerId = @event.OwnerId;
        CreatedAt = @event.OccurredOn;
    }
}