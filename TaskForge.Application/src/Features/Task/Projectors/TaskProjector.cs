using MediatR;
using TaskForge.Application.Interfaces;
using TaskForge.Domain.Aggregates.BoardTask.Events;
using TaskForge.Domain.ReadModels;

namespace TaskForge.Application.Features.Tasks.Projectors;

public class TaskProjector :
    INotificationHandler<TaskCreatedEvent>,
    INotificationHandler<TaskMovedEvent>
{
    private readonly IReadRepository<BoardReadModel> _boardRepository;

    public TaskProjector(IReadRepository<BoardReadModel> boardRepository)
    {
        _boardRepository = boardRepository;
    }

    // Inserts a new task document straight into its designated starter Column list
    public async Task Handle(TaskCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Find the board that owns the column specified in the event
        var boards = await _boardRepository.FindAsync(b => b.Columns.Any(c => c.Id == notification.ColumnId));
        var board = boards.FirstOrDefault();
        if (board == null) return;

        var targetColumn = board.Columns.First(c => c.Id == notification.ColumnId);

        targetColumn.Tasks.Add(new TaskReadModel
        {
            Id = notification.TaskId,
            Title = notification.Title,
            UpdatedAt = notification.OccurredOn
        });

        await _boardRepository.UpsertAsync(board.Id, board);
    }

    // Atomically pulls a task card out of its old column array and pushes it to the target column array
    public async Task Handle(TaskMovedEvent notification, CancellationToken cancellationToken)
    {
        // Find the board matching the source column tracking criteria
        var boards = await _boardRepository.FindAsync(b => b.Columns.Any(c => c.Id == notification.OldColumnId));
        var board = boards.FirstOrDefault();
        if (board == null) return;

        // 1. Locate and extract task tracking context from the source column
        var sourceColumn = board.Columns.First(c => c.Id == notification.OldColumnId);
        var taskCard = sourceColumn.Tasks.FirstOrDefault(t => t.Id == notification.TaskId);
        if (taskCard == null) return;

        sourceColumn.Tasks.Remove(taskCard);

        // 2. Inject task tracking data directly into the destination target column array
        var destinationColumn = board.Columns.FirstOrDefault(c => c.Id == notification.NewColumnId);
        if (destinationColumn != null)
        {
            taskCard.UpdatedAt = notification.OccurredOn;
            destinationColumn.Tasks.Add(taskCard);
        }

        // 3. Persist the updated flattened document tree safely to MongoDB
        await _boardRepository.UpsertAsync(board.Id, board);
    }
}