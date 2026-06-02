using TaskForge.Domain.Aggregates.User.Events;
using TaskForge.Domain.Events;
using TaskForge.Domain.Primitives;
using TaskForge.Domain.Services;

namespace TaskForge.Domain.Aggregates.User;

public class UserAggregate : AggregateRoot
{
    // Aggregate State Properties
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    // Required parameterless constructor for rehydration history streams
    private UserAggregate() { }

    /// <summary>
    /// Factory pattern to safely instantiate a new User stream with validated, hashed details.
    /// </summary>
    public static UserAggregate Register(
        Guid userId,
        string email,
        string rawPassword,
        string firstName,
        string lastName,
        IPasswordHasher passwordHasher)
    {
        // Guard Clauses / Domain Validations
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            throw new ArgumentException("A valid email address is required.", nameof(email));

        if (string.IsNullOrWhiteSpace(rawPassword) || rawPassword.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters long.", nameof(rawPassword));

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("First name and Last name cannot be empty.");

        // Hash the password using our domain service abstraction boundary
        var hashed = passwordHasher.HashPassword(rawPassword);

        var user = new UserAggregate();

        user.RaiseEvent(new UserRegisteredEvent(
            userId,
            email.ToLowerInvariant().Trim(),
            hashed,
            firstName.Trim(),
            lastName.Trim()
        ));

        return user;
    }

    /// <summary>
    /// Domain validation rule verifying incoming login parameters.
    /// </summary>
    public bool ValidatePassword(string rawPassword, IPasswordHasher passwordHasher)
    {
        if (!IsActive) return false;
        return passwordHasher.VerifyPassword(rawPassword, this.PasswordHash);
    }

    // =========================================================================
    // State Routing - Strongly Typed Internal Transitions
    // =========================================================================

    private void When(UserRegisteredEvent @event)
    {
        Id = @event.AggregateId;
        Email = @event.Email;
        PasswordHash = @event.PasswordHash;
        FirstName = @event.FirstName;
        LastName = @event.LastName;
        IsActive = true; // Users default to active upon registration
    }

    public static UserAggregate Rehydrate(IEnumerable<DomainEvent> history)
    {
        var user = new UserAggregate();
        user.LoadFromHistory(history);
        return user;
    }
}