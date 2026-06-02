using MediatR;
using TaskForge.Application.Interfaces;
using TaskForge.Domain.Aggregates.Project;

namespace TaskForge.Application.Features.Projects.Commands;

public record CreateProjectCommand(string Name, string Description, Guid OwnerId) : IRequest<Guid>;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Guid>
{
    private readonly IEventStoreRepository _eventStore;

    public CreateProjectCommandHandler(IEventStoreRepository eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var projectId = Guid.NewGuid();

        // Instantiate aggregate (validates internal business rules and stages ProjectCreatedEvent)
        var project = ProjectAggregate.Create(projectId, request.Name, request.Description, request.OwnerId);

        // Commit append-only logs down to PostgreSQL
        await _eventStore.AppendEventsAsync(project);

        return projectId;
    }
}