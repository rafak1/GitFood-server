using System.Net;
using Server.Logic.Abstract;

public static class ResultEnumToStatusCodeMapper
{
    public static int MapStatusCodeForResultEnum(this ResultEnum result)
        => result switch
            {
                ResultEnum.OK => (int)HttpStatusCode.OK,
                ResultEnum.NotFound => (int)HttpStatusCode.NotFound,
                ResultEnum.Conflict => (int)HttpStatusCode.Conflict,
                ResultEnum.Unauthorizated => (int)HttpStatusCode.Unauthorized,
                ResultEnum.BadRequest => (int)HttpStatusCode.BadRequest,
                _ => throw new InvalidOperationException("Unexpected Http status to map")
            };
}