using MediatR;
using TaskForge.Application.Interfaces;
using TaskForge.Domain.Aggregates.Board;

namespace TaskForge.Application.Features.Boards.Commands;

public record CreateBoardCommand(string Name, Guid ProjectId) : IRequest<Guid>;

public class CreateBoardCommandHandler : IRequestHandler<CreateBoardCommand, Guid>
{
    private readonly IEventStoreRepository _eventStore;

    public CreateBoardCommandHandler(IEventStoreRepository eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<Guid> Handle(CreateBoardCommand request, CancellationToken cancellationToken)
    {
        var boardId = Guid.NewGuid();

        // 1. Create the new Board aggregate context
        var board = BoardAggregate.Create(boardId, request.Name, request.ProjectId);

        // 2. Automatically inject standard Kanban starter columns via internal invariants
        board.AddColumn("To Do");
        board.AddColumn("In Progress");
        board.AddColumn("Done");

        // 3. Persist all staged events atomically to PostgreSQL
        await _eventStore.AppendEventsAsync(board);

        return boardId;
    }
}