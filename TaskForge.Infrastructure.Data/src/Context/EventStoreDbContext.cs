using Microsoft.EntityFrameworkCore;
using TaskForge.Infrastructure.Data.Entities;

namespace TaskForge.Infrastructure.Data.Context;

public class EventStoreDbContext : DbContext
{
    public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options) : base(options)
    {
    }

    public DbSet<EventEntity> StoredEvents => Set<EventEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EventEntity>(entity =>
        {
            // Define the table name
            entity.ToTable("StoredEvents");

            // 1. Setup Composite Key (StreamId + Version)
            entity.HasKey(e => new { e.StreamId, e.Version });

            // 2. Map payload columns to native PostgreSQL jsonb data types
            entity.Property(e => e.Data)
                .HasColumnType("jsonb")
                .IsRequired();

            entity.Property(e => e.Metadata)
                .HasColumnType("jsonb");

            entity.Property(e => e.EventType)
                .HasMaxLength(250)
                .IsRequired();

            // Optimizes query speeds when reading entire event histories sequentially
            entity.HasIndex(e => e.Timestamp);
        });
    }
}