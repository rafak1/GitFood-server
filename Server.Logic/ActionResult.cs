using Server.Logic.Abstract;

namespace Server.Logic;

internal class ManagerActionResult<T> : ManagerActionResult, IManagerActionResult<T>
{
    public ManagerActionResult() : base() {}

    public ManagerActionResult(T result) : base()
    {
        Result = result;
    }

    public ManagerActionResult(T result, ResultEnum resultEnum) : base(resultEnum)
    {
        Result = result;
    }

    public ManagerActionResult(T result, ResultEnum resultEnum, string message) : base(resultEnum, message)
    {
        Result = result;
    }

    public T Result { get; set; }
}

internal class ManagerActionResult : IManagerActionResult
{
    public ManagerActionResult(){}

    public ManagerActionResult(ResultEnum resultEnum)
    {
        EnumCode = resultEnum;
    }

    public ManagerActionResult(ResultEnum resultEnum, string message)
    {
        EnumCode = resultEnum;
        ErrorMessage = message;
    }

    public ResultEnum EnumCode { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}