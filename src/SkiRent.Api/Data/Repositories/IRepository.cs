using System.Linq.Expressions;

namespace SkiRent.Api.Data.Repositories;

public interface IRepository<T> where T : class
{
    public Task<T?> GetByIdAsync(int id);
    public Task<IEnumerable<T>> GetAllAsync();
    public Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
    public Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate);
    public Task AddAsync(T entity);
    public void Update(T entity);
    public void Delete(T entity);
    public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
}
