using Microsoft.AspNetCore.Mvc;
using Server.Logic.Abstract;

namespace Server.Controllers;

internal static class ManagerResultMapper
{
    public static IActionResult MapToActionResult(this IManagerActionResult managerActionResult)
        => MapResultCodeWithData(managerActionResult.EnumCode, managerActionResult.ErrorMessage);

    public static IActionResult MapToActionResult<T>(this IManagerActionResult<T> managerActionResult)
    {
        object data;
        data = managerActionResult.EnumCode == ResultEnum.OK 
            ? managerActionResult.Result 
            : managerActionResult.ErrorMessage;
        return MapResultCodeWithData(managerActionResult.EnumCode, data);
    }

    private static IActionResult MapResultCodeWithData(ResultEnum result, object data)
    {
        return new ObjectResult(data)
        {
            StatusCode = result.MapStatusCodeForResultEnum()
        };
    }
}