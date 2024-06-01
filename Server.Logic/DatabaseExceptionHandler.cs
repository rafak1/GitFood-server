
using Microsoft.EntityFrameworkCore;
using Server.Logic.Abstract;
using Server.Logic;
using Npgsql;

namespace Server.Logic;

internal class DatabaseExceptionHandler<T> : DatabaseExceptionHandlerBase, IDatabaseExceptionHandler<T>
{
    public async Task<IManagerActionResult<T>> HandleExceptionsAsync(Func<Task<IManagerActionResult<T>>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            return new ManagerActionResult<T>(default, ResultEnum.BadRequest, HandleInnerExceptions(ex));
        }
    }
}

internal class DatabaseExceptionHandler : DatabaseExceptionHandlerBase, IDatabaseExceptionHandler
{
    public async Task<IManagerActionResult> HandleExceptionsAsync(Func<Task<IManagerActionResult>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            return new ManagerActionResult(ResultEnum.BadRequest, HandleInnerExceptions(ex));
        }
    }
}