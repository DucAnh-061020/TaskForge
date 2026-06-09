using MediatR;
using TaskForge.Application.Interfaces;
using TaskForge.Domain.ReadModels;
using TaskForge.Domain.Aggregates.BoardTask.Events;

namespace TaskForge.Application.Features.Tasks.Projectors;

public class TaskProjector :
    INotificationHandler<TaskCreatedEvent>,
    INotificationHandler<TaskMovedEvent>
{
    private readonly IReadRepository<TaskReadModel> _taskRepository;

    public TaskProjector(IReadRepository<TaskReadModel> taskRepository)
    {
        _taskRepository = taskRepository;
    }

    // Inserts a new task document straight into its designated starter Column list
    public async Task Handle(TaskCreatedEvent notification, CancellationToken cancellationToken)
    {
        var model = new TaskReadModel
        {
            Id = notification.TaskId,
            Title = notification.Title,
            Description = notification.Description,
            AssignedTo = notification.AssignedUserId,
            CreatedBy = notification.CreatorUserId,
            ColumnId = notification.ColumnId,
            BoardId = notification.BoardId,
            UpdatedAt = notification.OccurredOn,
            CreatedAt = notification.OccurredOn
        };
        await _taskRepository.UpsertAsync(model.Id, model);
    }

    public async Task Handle(TaskMovedEvent notification, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(notification.TaskId);
        if (task == null)
        {
            throw new KeyNotFoundException($"Task with ID '{notification.TaskId}' could not be found.");
        }

        task.UpdatedAt = notification.OccurredOn;
        task.ColumnId = notification.NewColumnId;
        await _taskRepository.UpsertAsync(task.Id, task);
    }
}