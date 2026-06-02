using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskForge.Application.DTOs;
using TaskForge.Application.Interfaces;
using TaskForge.Domain.Aggregates.User;
using TaskForge.Domain.Services;

namespace TaskForge.Application.Features.Auth.Commands;

public record LoginUserCommand(string Email, string Password) : IRequest<AuthResult>;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResult>
{
    private readonly IEventStoreRepository _eventStore;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly IUserLookupService _userLookup;

    public LoginUserCommandHandler(IEventStoreRepository eventStore, IPasswordHasher passwordHasher, IConfiguration configuration, IUserLookupService userLookup)
    {
        _eventStore = eventStore;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _userLookup = userLookup;
    }

    public async Task<AuthResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Guid? userId = await _userLookup.GetUserIdByEmailAsync(request.Email);
            // 1. Resolve the User's Stream ID using an infrastructure lookup (e.g., from an Identity map or your read cache)
            if (userId == null)
            {
                throw new UnauthorizedAccessException("[1] Invalid email or password credentials.");
            }

            // 2. Fetch all historical security logs for this specific user stream
            var history = await _eventStore.ReadStreamAsync(userId.Value);
            if (history == null || history.Count == 0)
            {
                throw new UnauthorizedAccessException("[2] Invalid email or password credentials.");
            }

            // 3. Rehydrate the state properties of the aggregate root
            var user = UserAggregate.Rehydrate(history);

            // 4. Execute domain verification rule
            if (!user.ValidatePassword(request.Password, _passwordHasher))
            {
                throw new UnauthorizedAccessException("[3] Invalid email or password credentials.");
            }

            // 5. Generate Signed JSON Web Token
            var token = GenerateJwtToken(user);

            return new AuthResult(user.Id, user.Email, user.FirstName, user.LastName, token);
        }
        catch (Exception ex)
        {
            // Set a breakpoint here to see exactly what is broken!
            Console.WriteLine($"Login crashed raw: {ex.Message}");
            throw;
        }
    }

    private string GenerateJwtToken(UserAggregate user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"] ?? "YourSuperSecretMovingTasksJwtKeyChangeMe123!");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}