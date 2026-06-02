using Microsoft.EntityFrameworkCore;
using TaskForge.Application.Interfaces;
using TaskForge.Infrastructure.Data.Context;

namespace TaskForge.Infrastructure.Data.Security;

public class UserLookupService : IUserLookupService
{
    private readonly EventStoreDbContext _context; // Inject your existing EF DbContext

    public UserLookupService(EventStoreDbContext context)
    {
        _context = context;
    }

    private class UserLookupResult
    {
        public Guid StreamId { get; set; }
    }
    public async Task<Guid?> GetUserIdByEmailAsync(string email)
    {
        var normalizedEmail = email.ToLowerInvariant();

        var result = await _context.Database
            .SqlQueryRaw<UserLookupResult>("""
                SELECT "StreamId"
                FROM public."StoredEvents" 
                WHERE "EventType" LIKE '%UserRegisteredEvent%' 
                  AND LOWER("Data"->>'Email') = {0} 
                LIMIT 1
                """, normalizedEmail)
            .FirstOrDefaultAsync();

        return result?.StreamId;
    }
}