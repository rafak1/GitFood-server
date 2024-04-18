using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Server.Database.Abstract;

namespace Server.Database;

internal class InternalDbSet<T> : IDbSet<T> where T : class
{
    private readonly DbSet<T> _entity;

    public InternalDbSet(DbSet<T> entity)
    {
        _entity = entity ?? throw new ArgumentNullException(nameof(entity));
    }

    public async Task AddAsync(T entity) 
    {
        await _entity.AddAsync(entity);
    }

    public IEnumerable<T> Where(Func<T, bool> func)
    {
        return _entity.Where(func);
    }

    public async Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default(CancellationToken))
        => await _entity.FirstOrDefaultAsync(cancellationToken);

    public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> func, CancellationToken cancellationToken = default(CancellationToken))
        => await _entity.FirstOrDefaultAsync(func, cancellationToken);
}