namespace TaskForge.Domain.Exceptions;
public class ConcurrencyException : Exception
{
    public ConcurrencyException(Guid streamId, int expectedVersion, int actualVersion)
        : base($"Concurrency conflict detected on stream '{streamId}'. Expected Version: {expectedVersion}, but Database Version is: {actualVersion}.")
    {
    }
}