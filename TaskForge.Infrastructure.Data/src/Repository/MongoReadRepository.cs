using MongoDB.Driver;
using System.Linq.Expressions;
using TaskForge.Application.Interfaces;
using TaskForge.Infrastructure.Data.Context;

namespace TaskForge.Infrastructure.Data.Repository;
public class MongoReadRepository<TModel> : IReadRepository<TModel> where TModel : class
{
    private readonly IMongoCollection<TModel> _collection;

    public MongoReadRepository(MongoDbContext dbContext)
    {
        // Dynamically infer collection names using reflection based on model type structures
        var collectionName = typeof(TModel).Name.Replace("ReadModel", "s");

        // Alternative lookup fallback if you have specialized collections registered inside MongoDbContext
        var property = typeof(MongoDbContext).GetProperty(collectionName);
        if (property != null)
        {
            _collection = (IMongoCollection<TModel>)property.GetValue(dbContext)!;
        }
        else
        {
            // If property is missing, fetch a default runtime raw reference collection definition string
            var database = (IMongoDatabase)typeof(MongoDbContext)
                .GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .GetValue(dbContext)!;
            _collection = database.GetCollection<TModel>(collectionName);
        }
    }

    public async Task<TModel?> GetByIdAsync(Guid id)
    {
        // Filter expects an internal document element matching standard identifier properties
        var filter = Builders<TModel>.Filter.Eq("Id", id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<TModel>> FindAsync(Expression<Func<TModel, bool>> predicate)
    {
        return await _collection.Find(predicate).ToListAsync();
    }

    public async Task UpsertAsync(Guid id, TModel model)
    {
        var filter = Builders<TModel>.Filter.Eq("Id", id);

        // If matches existing stream target overwrite completely, else inject fresh document
        await _collection.ReplaceOneAsync(
            filter,
            model,
            new ReplaceOptions { IsUpsert = true });
    }

    public async Task DeleteAsync(Guid id)
    {
        var filter = Builders<TModel>.Filter.Eq("Id", id);
        await _collection.DeleteOneAsync(filter);
    }
}