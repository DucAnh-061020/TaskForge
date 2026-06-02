using MediatR;
using TaskForge.Application.Interfaces;
using TaskForge.Domain.Aggregates.BoardTask;

namespace TaskForge.Application.Features.Tasks.Commands;

public record MoveTaskCommand(Guid TaskId, Guid TargetColumnId) : IRequest;

public class MoveTaskCommandHandler : IRequestHandler<MoveTaskCommand>
{
    private readonly IEventStoreRepository _eventStore;

    public MoveTaskCommandHandler(IEventStoreRepository eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task Handle(MoveTaskCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch entire historical event stream for this specific task out of Postgres
        var history = await _eventStore.ReadStreamAsync(request.TaskId);
        if (history == null || !history.Any())
        {
            throw new KeyNotFoundException($"Task history stream with ID '{request.TaskId}' could not be resolved.");
        }

        // 2. Rehydrate state properties by replaying history through internal 'When' routers
        var task = BoardTaskAggregate.Rehydrate(history);

        // 3. Process business execution rules (stages TaskMovedEvent)
        task.Move(request.TargetColumnId);

        // 4. Save updates to PostgreSQL. 
        // NOTE: Throws ConcurrencyException automatically if another user moved this card first!
        await _eventStore.AppendEventsAsync(task);
    }
}