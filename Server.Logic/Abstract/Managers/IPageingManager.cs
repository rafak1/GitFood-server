namespace Server.Logic.Abstract.Managers;

internal interface IPageingManager
{
    IQueryable<T> GetPagedInfo<T>(IQueryable<T> values, int page, int pageSize);
}