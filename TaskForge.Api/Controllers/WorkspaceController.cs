using Microsoft.AspNetCore.Mvc;
using MediatR;
using TaskForge.Application.Features.Workspaces.Queries;

namespace TaskForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkspaceController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkspaceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserWorkspace(Guid userId)
    {
        var query = new GetUserWorkspaceQuery(userId);
        var workspaceDetails = await _mediator.Send(query, HttpContext.RequestAborted);

        if (workspaceDetails == null)
        {
            return NotFound();
        }

        return Ok(workspaceDetails);
    }
}