using MediatR;
using TaskForge.Application.Interfaces;
using TaskForge.Application.ReadModels;

namespace TaskForge.Application.Features.Workspaces.Queries;

public record GetUserWorkspaceQuery(Guid UserId) : IRequest<UserWorkspaceReadModel>;

public class GetUserWorkspaceQueryHandler : IRequestHandler<GetUserWorkspaceQuery, UserWorkspaceReadModel>
{
    private readonly IReadRepository<UserWorkspaceReadModel> _workspaceRepository;

    public GetUserWorkspaceQueryHandler(IReadRepository<UserWorkspaceReadModel> workspaceRepository)
    {
        _workspaceRepository = workspaceRepository;
    }

    public async Task<UserWorkspaceReadModel> Handle(GetUserWorkspaceQuery request, CancellationToken cancellationToken)
    {
        var workspaceView = await _workspaceRepository.GetByIdAsync(request.UserId);

        if (workspaceView == null)
        {
            throw new KeyNotFoundException($"Workspace context view for User ID '{request.UserId}' is empty or uninitialized.");
        }

        return workspaceView;
    }
}