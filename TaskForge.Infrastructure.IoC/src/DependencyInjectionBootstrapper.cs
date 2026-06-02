using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskForge.Application.Interfaces;
using TaskForge.Domain.Services;
using TaskForge.Infrastructure.Data;
using TaskForge.Infrastructure.Data.Context;
using TaskForge.Infrastructure.Data.Repository;
using TaskForge.Infrastructure.Data.Security;

namespace TaskForge.Infrastructure.IoC;

public static class DependencyInjectionBootstrapper
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        // PostgreSQL Connection
        var connectionString = configuration.GetConnectionString("PostgresConnection");
        services.AddDbContext<EventStoreDbContext>(options =>
           options.UseNpgsql(connectionString, b =>
               b.MigrationsAssembly("TaskForge.Infrastructure.Data")));

        // System Repositories
        services.AddScoped<IEventStoreRepository, EventStoreRepository>();

        // MongoDB Context Wireup
        services.AddSingleton<MongoDbContext>();

        // Register Open Generic Read Repositories
        services.AddScoped(typeof(IReadRepository<>), typeof(MongoReadRepository<>));

        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.DTOs.AuthResult).Assembly));

        return services;
    }
}