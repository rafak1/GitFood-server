using Server.Logic.Abstract.Managers;

internal class PageingManager : IPageingManager
{
    public IQueryable<T> GetPagedInfo<T>(IQueryable<T> values, int page, int pageSize)
        => values.Skip((page - 1) * pageSize).Take(pageSize);
}