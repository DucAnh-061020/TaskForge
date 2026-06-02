using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TaskForge.Application.Interfaces;
using TaskForge.Domain.Events;
using TaskForge.Domain.Primitives;
using TaskForge.Infrastructure.Data.Context;
using TaskForge.Infrastructure.Data.Entities;

namespace TaskForge.Infrastructure.Data;
public class EventStoreRepository : IEventStoreRepository
{
    private readonly EventStoreDbContext _context;
    private readonly IMediator _mediator;

    public EventStoreRepository(EventStoreDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    /// <summary>
    /// Saves all uncommitted events tracked by an aggregate root down to PostgreSQL.
    /// </summary>
    public async Task AppendEventsAsync(AggregateRoot aggregate)
    {
        var pendingEvents = aggregate.GetUncommittedEvents();
        if (!pendingEvents.Any()) return;

        // Calculate expected version tracking. 
        // If it's a new aggregate, expected version equals total pending events minus 1.
        int totalEvents = pendingEvents.Count;
        int initialExpectedVersion = aggregate.Version - totalEvents + 1;

        foreach (var @event in pendingEvents)
        {
            var jsonOptions = new JsonSerializerOptions { WriteIndented = false };

            var storedEvent = new EventEntity
            {
                StreamId = @event.AggregateId,
                Version = @event.Version,
                EventType = @event.GetType().AssemblyQualifiedName ?? @event.GetType().Name,
                Timestamp = @event.OccurredOn,
                Data = JsonSerializer.Serialize(@event, @event.GetType(), jsonOptions),
                Metadata = JsonSerializer.Serialize(new { MachineName = Environment.MachineName }) // Expandable for UserContext tracking
            };

            _context.StoredEvents.Add(storedEvent);
        }

        await _context.SaveChangesAsync();

        foreach (var @event in pendingEvents)
        {
            await _mediator.Publish(@event);
        }

        aggregate.ClearUncommittedEvents();
    }

    /// <summary>
    /// Rehydrates an aggregate history stream by reading all past events sequentially.
    /// </summary>
    public async Task<List<DomainEvent>> ReadStreamAsync(Guid streamId)
    {
        var dbEvents = await _context.StoredEvents
            .Where(e => e.StreamId == streamId)
            .OrderBy(e => e.Version)
            .AsNoTracking()
            .ToListAsync();

        var domainEvents = new List<DomainEvent>();

        foreach (var dbEvent in dbEvents)
        {
            var eventType = Type.GetType(dbEvent.EventType);
            if (eventType == null)
            {
                throw new InvalidOperationException($"Could not resolve domain event system type definition for: '{dbEvent.EventType}'");
            }

            var domainEvent = JsonSerializer.Deserialize(dbEvent.Data, eventType) as DomainEvent;
            if (domainEvent != null)
            {
                domainEvents.Add(domainEvent);
            }
        }

        return domainEvents;
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        // 23505 is the explicit standard PostgreSQL error code for Unique/Primary Key Violations
        return ex.InnerException is Npgsql.PostgresException pex && pex.SqlState == "23505";
    }
}