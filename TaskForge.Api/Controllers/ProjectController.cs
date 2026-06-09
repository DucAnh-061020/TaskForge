using Microsoft.AspNetCore.Mvc;
using MediatR;
using TaskForge.Application.Features.Projects.Queries;
using TaskForge.Application.Features.Projects.Commands;

namespace TaskForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("createproject")]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectCommand command)
    {
        var projectId = await _mediator.Send(command);

        return Ok(new { id = projectId });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(Guid id)
    {
        var query = new GetProjectQuery(id);
        var projectDetails = await _mediator.Send(query);

        if (projectDetails == null)
        {
            return NotFound();
        }

        return Ok(projectDetails);
    }
}