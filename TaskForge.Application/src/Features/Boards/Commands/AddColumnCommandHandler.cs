using MediatR;
using TaskForge.Application.Interfaces;
using TaskForge.Domain.Aggregates.Board;

namespace TaskForge.Application.Features.Boards.Commands;

public record AddColumnCommand(Guid BoardId, string ColumnName) : IRequest;

public class AddColumnCommandHandler : IRequestHandler<AddColumnCommand>
{
    private readonly IEventStoreRepository _eventStore;

    public AddColumnCommandHandler(IEventStoreRepository eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task Handle(AddColumnCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch entire event history log for this specific board stream from Postgres
        var history = await _eventStore.ReadStreamAsync(request.BoardId);
        if (history == null || !history.Any())
            throw new KeyNotFoundException($"Board with ID '{request.BoardId}' could not be found.");

        // 2. Rehydrate the aggregate stream through its internal state routers
        var board = BoardAggregate.Rehydrate(history);

        // 3. Execute domain logic mutation (Validates duplicates inside aggregate)
        board.AddColumn(request.ColumnName);

        // 4. Commit newly staged append log events back into PostgreSQL
        await _eventStore.AppendEventsAsync(board);
    }
}