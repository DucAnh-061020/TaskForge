using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskForge.Application.DTOs;
using TaskForge.Application.Features.Auth.Commands;

namespace TaskForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    // Inject MediatR to dispatch commands to their respective handlers
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers a new user inside the application.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        // Dispatches the command to RegisterUserCommandHandler
        var userId = await _mediator.Send(command);

        return Ok(new { Id = userId });
    }

    /// <summary>
    /// Authenticates a user and returns a signed JWT token.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResult))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        try
        {
            // Dispatches the command to LoginUserCommandHandler
            var authResult = await _mediator.Send(command);

            return Ok(authResult);
        }
        catch (UnauthorizedAccessException ex)
        {
            // Catches validation failures thrown inside your command handler
            return Unauthorized(new { message = ex.Message });
        }
    }
}