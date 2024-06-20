using Microsoft.AspNetCore.Mvc;
using Server.Logic.Abstract.Token;

public class BaseController : Controller 
{
    private const int _bearerOffset = 7;
    private readonly ITokenStorage _tokenStorage;

    public BaseController(ITokenStorage tokenStorage)
    {
        _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
    }

    protected string GetUser() 
        => _tokenStorage.GetUser(((string)Request.Headers.Authorization)[_bearerOffset..]) ?? throw new UserNotFoundException();


    protected void RemoveUser() 
        => _tokenStorage.RemoveUser(((string)Request.Headers.Authorization)[_bearerOffset..]);
}