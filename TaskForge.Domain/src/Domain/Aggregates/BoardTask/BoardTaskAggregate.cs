using TaskForge.Domain.Aggregates.BoardTask.Events;
using TaskForge.Domain.Events;
using TaskForge.Domain.Primitives;

namespace TaskForge.Domain.Aggregates.BoardTask;

public class BoardTaskAggregate : AggregateRoot
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid ColumnId { get; private set; }
    public Guid AssignedUserId { get; private set; }
    public DateTime LastModifiedAt { get; private set; }

    private BoardTaskAggregate() { }

    // 1. Factory matching the official 5-argument constructor
    public static BoardTaskAggregate Create(Guid taskId, string title, string description, Guid columnId, Guid assignedUserId)
    {
        var task = new BoardTaskAggregate();

        task.RaiseEvent(new TaskCreatedEvent(
            taskId,
            title.Trim(),
            description?.Trim() ?? string.Empty,
            columnId,
            assignedUserId
        ));

        return task;
    }

    // 2. Move method matching the official 3-argument constructor (OldColumnId, NewColumnId)
    public void Move(Guid targetColumnId)
    {
        if (targetColumnId == Guid.Empty)
            throw new ArgumentException("Destination target column identifier cannot be empty.", nameof(targetColumnId));

        if (ColumnId == targetColumnId) return;

        // Passes 3 arguments: TaskId, Current Column ID (Old), and Target Column ID (New)
        RaiseEvent(new TaskMovedEvent(this.Id, this.ColumnId, targetColumnId));
    }

    public static BoardTaskAggregate Rehydrate(IEnumerable<DomainEvent> history)
    {
        var task = new BoardTaskAggregate();
        task.LoadFromHistory(history);
        return task;
    }

    // =========================================================================
    // State Routing - Updates locally via correct arguments
    // =========================================================================

    private void When(TaskCreatedEvent @event)
    {
        Id = @event.AggregateId;
        Title = @event.Title;
        Description = @event.Description;
        ColumnId = @event.ColumnId;
        AssignedUserId = @event.AssignedUserId;
        LastModifiedAt = @event.OccurredOn;
    }

    private void When(TaskMovedEvent @event)
    {
        ColumnId = @event.NewColumnId; // Links to the 3-argument property map
        LastModifiedAt = @event.OccurredOn;
    }
}