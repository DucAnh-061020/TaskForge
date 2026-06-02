using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using TaskForge.Application.ReadModels;

namespace TaskForge.Infrastructure.Data.Context;
public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        // Connection string matches the environment variable mapped in docker-compose
        var connectionString = configuration.GetConnectionString("MongoConnection");
        var mongoUrl = new MongoUrl(connectionString);

        var client = new MongoClient(mongoUrl);
        _database = client.GetDatabase(mongoUrl.DatabaseName ?? "TaskForgeReadDb");

        CreatePerformanceIndexes();
    }

    // Expose collection entry points for Read Models
    public IMongoCollection<UserWorkspaceReadModel> UserWorkspaces => _database.GetCollection<UserWorkspaceReadModel>("UserWorkspaces");
    public IMongoCollection<BoardDashboardReadModel> Boards => _database.GetCollection<BoardDashboardReadModel>("Boards");

    private void CreatePerformanceIndexes()
    {
        var emailIndex = Builders<UserWorkspaceReadModel>.IndexKeys.Ascending(u => u.UserEmail);
        UserWorkspaces.Indexes.CreateOne(new CreateIndexModel<UserWorkspaceReadModel>(emailIndex));

        // Multikey index to track a task across deeply nested arrays during MoveTask scenarios
        var embeddedTaskIndex = Builders<BoardDashboardReadModel>.IndexKeys.Ascending("Columns.Tasks.Id");
        Boards.Indexes.CreateOne(new CreateIndexModel<BoardDashboardReadModel>(embeddedTaskIndex));
    }
}