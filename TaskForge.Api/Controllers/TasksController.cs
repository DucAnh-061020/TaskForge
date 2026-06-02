using Microsoft.AspNetCore.Mvc;
using MediatR;
using TaskForge.Application.Features.Tasks.Commands;
using TaskForge.Application.Features.Tasks.Queries;

namespace TaskForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    private readonly IMediator _mediator;

    public TaskController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("createtask")]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskCommand command)
    {
        var taskId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTask), new { id = taskId }, taskId);
    }

    [HttpPost("movetask")]
    public async Task<IActionResult> MoveTask([FromBody] MoveTaskCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(Guid id)
    {
        var query = new GetTaskQuery(id);
        var taskDetails = await _mediator.Send(query);

        if (taskDetails == null)
        {
            return NotFound();
        }

        return Ok(taskDetails);
    }
}