using System.Linq.Expressions;

namespace Server.Database.Abstract;

public interface IDbSet<T> where T : class
{
    Task AddAsync(T entity);
    IEnumerable<T> Where(Func<T, bool> func);
    public Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default(CancellationToken));
    public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> func, CancellationToken cancellationToken = default(CancellationToken));
}