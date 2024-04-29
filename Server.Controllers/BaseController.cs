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

    public string GetUser(string token) 
    {
        return _tokenStorage.GetUser(token[_bearerOffset..]);
    }
}