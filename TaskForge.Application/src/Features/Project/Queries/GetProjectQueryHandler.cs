using MediatR;
using TaskForge.Application.Features.Projects.Projectors;
using TaskForge.Application.Interfaces;

namespace TaskForge.Application.Features.Projects.Queries;

public record GetProjectQuery(Guid Id) : IRequest<ProjectReadModel>;

public class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, ProjectReadModel>
{
    private readonly IReadRepository<ProjectReadModel> _projectRepository;

    public GetProjectQueryHandler(IReadRepository<ProjectReadModel> projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ProjectReadModel> Handle(GetProjectQuery request, CancellationToken cancellationToken)
    {
        var projectDetails = await _projectRepository.GetByIdAsync(request.Id);

        if (projectDetails == null)
        {
            throw new KeyNotFoundException($"The project with ID '{request.Id}' could not be found.");
        }

        return projectDetails;
    }
}