using TaskForge.Domain.Events;

namespace TaskForge.Domain.Primitives;

public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _uncommittedEvents = new();

    public Guid Id { get; protected set; }
    public int Version { get; private set; } = -1; // New streams start at version -1 (first event pushes to 0)

    public IReadOnlyCollection<IDomainEvent> GetUncommittedEvents() => _uncommittedEvents.AsReadOnly();
    public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

    /// <summary>
    /// Executed by Domain logic when creating a new structural transformation change.
    /// </summary>
    protected void RaiseEvent(DomainEvent @event)
    {
        // Push version increment rules forward
        @event.Version = Version + 1;

        // Router state application locally
        ApplyEvent(@event);

        // Track change history to save back to PostgreSQL later
        _uncommittedEvents.Add(@event);
    }

    /// <summary>
    /// Replays historical database entities back down the wire to rebuild state tracking.
    /// </summary>
    public void LoadFromHistory(IEnumerable<IDomainEvent> history)
    {
        foreach (var @event in history)
        {
            ApplyEvent(@event);
            Version = @event.Version; // Lock version status to match historical ledger
        }
    }

    private void ApplyEvent(IDomainEvent @event)
    {
        // Strongly-typed reflection routing. Looks for private void When(TEvent e) inside your concrete classes.
        var method = GetType().GetMethod("When",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
            new[] { @event.GetType() });

        if (method != null)
        {
            method.Invoke(this, new object[] { @event });
        }
        else
        {
            throw new InvalidOperationException($"The aggregate state router 'When' method was not found for event: {@event.GetType().Name}");
        }
    }

    //public static T LoadFromHistory<T>(IEnumerable<DomainEvent> history) where T : AggregateRoot, new()
    //{
    //    var aggregate = new T();
    //    aggregate.LoadFromHistory(history);
    //    return aggregate;
    //}
}