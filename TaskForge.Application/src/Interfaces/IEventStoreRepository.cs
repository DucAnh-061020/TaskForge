using TaskForge.Domain.Events;
using TaskForge.Domain.Primitives;

namespace TaskForge.Application.Interfaces;
public interface IEventStoreRepository
{
    Task AppendEventsAsync(AggregateRoot aggregate);
    Task<List<DomainEvent>> ReadStreamAsync(Guid streamId);
}
