using System.Linq.Expressions;

namespace TaskForge.Application.Interfaces;
public interface IReadRepository<TModel> where TModel : class
{
    Task<TModel?> GetByIdAsync(Guid id);
    Task<IEnumerable<TModel>> FindAsync(Expression<Func<TModel, bool>> predicate);
    Task UpsertAsync(Guid id, TModel model);
    Task DeleteAsync(Guid id);
}