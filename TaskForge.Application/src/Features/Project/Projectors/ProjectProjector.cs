using MediatR;
using TaskForge.Application.Interfaces;
using TaskForge.Domain.Aggregates.Project.Events;
using TaskForge.Domain.ReadModels;

namespace TaskForge.Application.Features.Projects.Projectors;

public class ProjectProjector : INotificationHandler<ProjectCreatedEvent>
{
    private readonly IReadRepository<ProjectReadModel> _projectRepository;

    public ProjectProjector(IReadRepository<ProjectReadModel> projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task Handle(ProjectCreatedEvent notification, CancellationToken cancellationToken)
    {
        var model = new ProjectReadModel
        {
            Id = notification.ProjectId,
            Name = notification.Name,
            Description = notification.Description,
            OwnerId = notification.OwnerId,
            CreatedAt = notification.OccurredOn
        };

        await _projectRepository.UpsertAsync(model.Id, model);
    }
}