using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Logic.Abstract.Managers;
using Server.Logic.Abstract.Token;
using Server.Logic.Token;
using Server.ViewModels.Login;

namespace Server.Controllers;

[ApiController]
public class LoginController : BaseController
{

    private const string _controllerRoute = "/login";
    private readonly ILoginManager _loginManager;

    public LoginController(ILoginManager loginManager, ITokenStorage tokenStorage) : base(tokenStorage)
    {
        _loginManager = loginManager ?? throw new ArgumentNullException(nameof(loginManager));
    }

    [HttpPost]
    [AllowAnonymous]
    [Route($"{_controllerRoute}")]
    public async Task<IActionResult> Login(LoginViewModel login)
        => (await _loginManager.LoginAsync(login)).MapToActionResult();

    [HttpPost]
    [AllowAnonymous]
    [Route($"{_controllerRoute}/register")]
    public async Task<IActionResult> Register(LoginViewModel login) 
        => (await _loginManager.RegisterAsync(login)).MapToActionResult();

    [HttpDelete]
    [Route($"{_controllerRoute}/signOut")]
    public async Task<IActionResult> signOut()
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);
        RemoveUser(Request.Headers.Authorization);
        return Ok();
    }
}
