using Microsoft.AspNetCore.Mvc;
using MediatR;
using TaskForge.Application.Features.Boards.Commands;
using TaskForge.Application.Features.Boards.Queries;

namespace TaskForge.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardController : ControllerBase
{
    private readonly IMediator _mediator;

    public BoardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("createboard")]
    public async Task<IActionResult> CreateBoard([FromBody] CreateBoardCommand command)
    {
        var boardId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetBoard), new { id = boardId }, boardId);
    }

    [HttpPost("{id}/columns")]
    public async Task<IActionResult> AddColumn(Guid id, [FromBody] AddColumnCommand command)
    {
        await _mediator.Send(new AddColumnCommand(id, command.ColumnName));
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBoard(Guid id)
    {
        var query = new GetBoardDashboardQuery(id);
        var boardDetails = await _mediator.Send(query, HttpContext.RequestAborted);

        if (boardDetails == null)
        {
            return NotFound();
        }

        return Ok(boardDetails);
    }
}