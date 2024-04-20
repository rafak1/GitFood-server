namespace Server.Logic.Abstract;

public interface IManagerActionResult<T> : IManagerActionResult
{
    public T Result { get; }
}

public interface IManagerActionResult
{
    public ResultEnum EnumCode { get; }
    public string ErrorMessage { get; }
}