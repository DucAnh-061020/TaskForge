using TaskForge.Domain.Events;

namespace TaskForge.Domain.Aggregates.User.Events;
public record UserRegisteredEvent(
    Guid UserId,
    string Email,
    string PasswordHash,
    string FirstName,
    string LastName) : DomainEvent(UserId);