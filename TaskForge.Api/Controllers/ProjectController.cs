using Microsoft.AspNetCore.Mvc;
using MediatR;
using TaskForge.Application.Features.Projects.Queries;

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