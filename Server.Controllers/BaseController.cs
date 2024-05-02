using Microsoft.AspNetCore.Mvc;
using Server.Logic.Abstract.Token;

public class BaseController : Controller 
{
    protected const int _bearerOffset = 7;

    private readonly ITokenStorage _tokenStorage;

    public BaseController(ITokenStorage tokenStorage)
    {
        _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
    }

    protected string GetUser(string token) 
    {
        return _tokenStorage.GetUser(token[_bearerOffset..]);
    }

    protected void RemoveUser(string token) 
    {
        _tokenStorage.RemoveUser(token[_bearerOffset..]);
    }
}