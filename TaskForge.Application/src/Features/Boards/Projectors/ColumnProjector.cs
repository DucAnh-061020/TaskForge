using MediatR;
using TaskForge.Application.Interfaces;
using TaskForge.Domain.Aggregates.Board.Events;
using TaskForge.Domain.ReadModels;

namespace TaskForge.Application.Features.Boards.Projectors;
public class ColumnProjector : INotificationHandler<ColumnAddedEvent>
{
    private readonly IReadRepository<ColumnReadModel> _columnRepository;

    public ColumnProjector(IReadRepository<ColumnReadModel> columnRepository)
    {
        _columnRepository = columnRepository;
    }

    public async Task Handle(ColumnAddedEvent notification, CancellationToken cancellationToken)
    {
        var board = await _columnRepository.GetByIdAsync(notification.BoardId);
        if (board == null) return;

        var newColumn = new ColumnReadModel
        {
            Id = notification.ColumnId,
            Name = notification.Name,
            BoardId = notification.BoardId,
            CreatedAt = notification.OccurredOn,
        };

        await _columnRepository.UpsertAsync(board.Id, board);
    }
}
