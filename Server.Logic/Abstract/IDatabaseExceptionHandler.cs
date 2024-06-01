
using Server.Logic.Abstract;

internal interface IDatabaseExceptionHandler<T>
{
    Task<IManagerActionResult<T>> HandleExceptionsAsync(Func<Task<IManagerActionResult<T>>> action);
}

internal interface IDatabaseExceptionHandler
{
    Task<IManagerActionResult> HandleExceptionsAsync(Func<Task<IManagerActionResult>> action);
}