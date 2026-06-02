using MediatR;
using TaskForge.Application.Interfaces;
using TaskForge.Domain.Aggregates.User;
using TaskForge.Domain.Services;

namespace TaskForge.Application.Features.Auth.Commands;
public record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName) : IRequest<Guid>;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IEventStoreRepository _eventStore;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(IEventStoreRepository eventStore, IPasswordHasher passwordHasher)
    {
        _eventStore = eventStore;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.NewGuid();

        // Instantiate UserAggregate executing inner domain validations and hashing
        var user = UserAggregate.Register(
            userId,
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            _passwordHasher
        );

        // Commit events to PostgreSQL
        await _eventStore.AppendEventsAsync(user);

        return userId;
    }
}