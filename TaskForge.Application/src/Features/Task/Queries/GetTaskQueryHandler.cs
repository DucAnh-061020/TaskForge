using MediatR;
using TaskForge.Application.Interfaces;
using TaskForge.Domain.ReadModels;

namespace TaskForge.Application.Features.Tasks.Queries;

public record GetTaskQuery(Guid Id) : IRequest<TaskReadModel>;

public class GetTaskQueryHandler : IRequestHandler<GetTaskQuery, TaskReadModel>
{
    private readonly IReadRepository<TaskReadModel> _taskRepository;

    public GetTaskQueryHandler(IReadRepository<TaskReadModel> taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskReadModel> Handle(GetTaskQuery request, CancellationToken cancellationToken)
    {
        var taskDetails = await _taskRepository.GetByIdAsync(request.Id);

        if (taskDetails == null)
        {
            throw new KeyNotFoundException($"The task with ID '{request.Id}' could not be found.");
        }

        return taskDetails;
    }
}