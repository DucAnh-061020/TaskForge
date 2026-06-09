using MediatR;
using TaskForge.Application.Interfaces;
using TaskForge.Domain.Aggregates.BoardTask;

namespace TaskForge.Application.Features.Tasks.Commands;

public record CreateTaskCommand(
    string Title,
    string Description,
    Guid ColumnId,
    Guid AssignedUserId,
    Guid CreaterUserId,
    Guid BoardId) : IRequest<Guid>;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
{
    private readonly IEventStoreRepository _eventStore;

    public CreateTaskCommandHandler(IEventStoreRepository eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var taskId = Guid.NewGuid();

        // Build the transactional card element (stages TaskCreatedEvent)
        var task = BoardTaskAggregate.Create(
            taskId,
            request.Title,
            request.Description,
            request.ColumnId,
            request.AssignedUserId,
            request.CreaterUserId,
            request.BoardId
        );

        // Commit transaction trail to the Database
        await _eventStore.AppendEventsAsync(task);

        return taskId;
    }
}