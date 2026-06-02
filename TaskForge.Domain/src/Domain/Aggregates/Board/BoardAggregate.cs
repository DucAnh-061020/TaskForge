using TaskForge.Domain.Aggregates.Board.Events;
using TaskForge.Domain.Events;
using TaskForge.Domain.Primitives;

namespace TaskForge.Domain.Aggregates.Board;

public class BoardAggregate : AggregateRoot
{
    // Aggregate State Properties
    public string Name { get; private set; } = string.Empty;
    public Guid ProjectId { get; private set; }

    // Internal tracked listing of columns belonging to this board context
    private readonly List<ColumnState> _columns = new();
    public IReadOnlyCollection<ColumnState> Columns => _columns.AsReadOnly();

    // Required parameterless constructor for historical rehydration
    private BoardAggregate() { }

    /// <summary>
    /// Factory pattern to instantiate a valid, rule-compliant Board stream.
    /// </summary>
    public static BoardAggregate Create(Guid boardId, string name, Guid projectId)
    {
        if (boardId == Guid.Empty)
            throw new ArgumentException("Board Identifier cannot be empty.", nameof(boardId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Board name cannot be empty or whitespace.", nameof(name));

        if (projectId == Guid.Empty)
            throw new ArgumentException("Board must be attached to a valid project.", nameof(projectId));

        var board = new BoardAggregate();
        board.RaiseEvent(new BoardCreatedEvent(boardId, name.Trim(), projectId));
        return board;
    }

    /// <summary>
    /// Domain Command logic to safely append a column while enforcing business invariants.
    /// </summary>
    public void AddColumn(string columnName)
    {
        if (string.IsNullOrWhiteSpace(columnName))
            throw new ArgumentException("Column name cannot be empty.", nameof(columnName));

        // Invariant Guard: Prevent duplicate columns on the same board
        if (_columns.Any(c => c.Name.Equals(columnName.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"A column named '{columnName}' already exists on this board.");

        var columnId = Guid.NewGuid();
        int nextOrder = _columns.Count; // Appends sequentially to the right side of the layout

        RaiseEvent(new ColumnAddedEvent(this.Id, columnId, columnName.Trim(), nextOrder));
    }

    public static BoardAggregate Rehydrate(IEnumerable<DomainEvent> history)
    {
        var board = new BoardAggregate();
        board.LoadFromHistory(history);
        return board;
    }

    // =========================================================================
    // State Routing - Strongly Typed Internal Transitions
    // =========================================================================

    private void When(BoardCreatedEvent @event)
    {
        Id = @event.AggregateId;
        Name = @event.Name;
        ProjectId = @event.ProjectId;
    }

    private void When(ColumnAddedEvent @event)
    {
        _columns.Add(new ColumnState(@event.ColumnId, @event.Name, @event.Order));
    }
}