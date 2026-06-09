using MediatR;
using TaskForge.Application.Interfaces;
using TaskForge.Domain.Aggregates.Board.Events;
using TaskForge.Domain.ReadModels;

namespace TaskForge.Application.Features.Boards.Projectors;

public class BoardProjector : INotificationHandler<BoardCreatedEvent>
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
            CreatedAt = notification.OccurredOn,
        };

        await _boardRepository.UpsertAsync(board.Id, board);
    }
}