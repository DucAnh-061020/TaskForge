using MediatR;
using TaskForge.Application.Interfaces;
using TaskForge.Domain.ReadModels;

namespace TaskForge.Application.Features.Boards.Queries;

public record GetBoardDashboardQuery(Guid BoardId) : IRequest<BoardReadModel>;

public class GetBoardDashboardQueryHandler : IRequestHandler<GetBoardDashboardQuery, BoardReadModel>
{
    private readonly IReadRepository<BoardReadModel> _boardRepository;
    public GetBoardDashboardQueryHandler(IReadRepository<BoardReadModel> boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<BoardReadModel> Handle(GetBoardDashboardQuery request, CancellationToken cancellationToken)
    {
        var boardDashboard = await _boardRepository.GetByIdAsync(request.BoardId);

        if (boardDashboard == null)
        {
            throw new KeyNotFoundException($"The materialized dashboard view for Board ID '{request.BoardId}' could not be found or has not been projected yet.");
        }
        return boardDashboard;
    }
}