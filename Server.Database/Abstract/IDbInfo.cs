namespace Server.Database.Abstract;

public interface IDbInfo : IDbTables
{
    Task<int> SaveChangesAsync();
    void Update<T>(T entity) where T : class;
}