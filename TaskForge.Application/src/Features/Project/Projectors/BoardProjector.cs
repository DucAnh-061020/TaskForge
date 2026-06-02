using MediatR;
using TaskForge.Application.Interfaces;
using TaskForge.Domain.Aggregates.Board.Events;
using TaskForge.Domain.ReadModels;

namespace TaskForge.Application.Features.Boards.Projectors;

public class BoardProjector :
    INotificationHandler<BoardCreatedEvent>,
    INotificationHandler<ColumnAddedEvent>
{
    private readonly IReadRepository<BoardReadModel> _boardRepository;

    public BoardProjector(IReadRepository<BoardReadModel> boardRepository)
    {
        _boardRepository = boardRepository;
    }

    // Handles initial Board initialization
    public async Task Handle(BoardCreatedEvent notification, CancellationToken cancellationToken)
    {
        var board = new BoardReadModel
        {
            Id = notification.BoardId,
            Name = notification.Name,
            ProjectId = notification.ProjectId,
            Columns = new System.Collections.Generic.List<ColumnReadModel>()
        };

        await _boardRepository.UpsertAsync(board.Id, board);
    }

    // Handles appending embedded columns safely inside the targeted Board document
    public async Task Handle(ColumnAddedEvent notification, CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetByIdAsync(notification.BoardId);
        if (board == null) return;

        var newColumn = new ColumnReadModel
        {
            Id = notification.ColumnId,
            Name = notification.Name,
            Tasks = new System.Collections.Generic.List<TaskReadModel>()
        };

        board.Columns.Add(newColumn);
        await _boardRepository.UpsertAsync(board.Id, board);
    }
}