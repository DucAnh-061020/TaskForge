using MediatR;
using TaskForge.Application.Interfaces;
using TaskForge.Application.ReadModels;

namespace TaskForge.Application.Features.Boards.Queries;

// The Query definition: expects a BoardId and returns the deeply nested dashboard layout view
public record GetBoardDashboardQuery(Guid BoardId) : IRequest<BoardDashboardReadModel>;

public class GetBoardDashboardQueryHandler : IRequestHandler<GetBoardDashboardQuery, BoardDashboardReadModel>
{
    private readonly IReadRepository<BoardDashboardReadModel> _boardRepository;

    public GetBoardDashboardQueryHandler(IReadRepository<BoardDashboardReadModel> boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<BoardDashboardReadModel> Handle(GetBoardDashboardQuery request, CancellationToken cancellationToken)
    {
        // Fetch the flattened document directly from MongoDB cache via your repository layer
        var boardDashboard = await _boardRepository.GetByIdAsync(request.BoardId);

        if (boardDashboard == null)
        {
            throw new KeyNotFoundException($"The materialized dashboard view for Board ID '{request.BoardId}' could not be found or has not been projected yet.");
        }

        // Returns everything (Columns, Subarrays, Tasks, and Assignees) in a single hop
        return boardDashboard;
    }
}