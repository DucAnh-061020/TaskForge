using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TaskForge.Infrastructure.Data.Context;
public class EventStoreDbContextFactory : IDesignTimeDbContextFactory<EventStoreDbContext>
{
    public EventStoreDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EventStoreDbContext>();

        // Hardcoded fallback connection string used ONLY during migration generation tasks locally
        var connectionString = "Host=localhost;Port=5432;Database=TaskForgeWriteDb;Username=taskforge_admin;Password=ChangeMeSecret2026!;";

        optionsBuilder.UseNpgsql(connectionString, b =>
            b.MigrationsAssembly("TaskForge.Infrastructure.Data"));

        return new EventStoreDbContext(optionsBuilder.Options);
    }
}